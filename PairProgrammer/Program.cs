using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using PairProgrammer.Functions;
using PairProgrammer.GptApi;

namespace PairProgrammer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

        var chatGptApi = new ChatGptApi(apiKey);
        IProgrammerInterface programmerInterface = new ProgrammerInterface();

        var messages = new List<Message> {
            new() { Role = Role.System, Content = GetSystemPrompt() },
            new() { Role = Role.User, Content = programmerInterface.GetMessage() }
        };

        var fs = new FileSystemAccess(args[0], new FileSystem());

        var functions = new[] {
            new ReadFileCommand(fs)
        };

        while (true) {
            var response = await GetChatGptResponseAsync(chatGptApi, messages, functions, programmerInterface, 10);
            
            var responseMessage = response.Choices.First().Message;
            messages.Add(responseMessage);

            if (responseMessage.FunctionCall == null) {
                programmerInterface.LogAiMessage(responseMessage.Content);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = programmerInterface.GetMessage()
                });
            }
            else {
                var stringData = JsonConvert.SerializeObject(responseMessage.FunctionCall);
                programmerInterface.LogAiMessage($"(FUNCTION): {stringData}");
                messages.Add(new Message {
                    Role = Role.Function,
                    Name = responseMessage.FunctionCall.Name,
                    Content = programmerInterface.GetMessage()
                });
            }
        }
    }
    
    
    public static async Task<ChatGptResponse> GetChatGptResponseAsync(ChatGptApi chatGptApi, 
                                                               IEnumerable<Message> messages,
                                                               IEnumerable<ICommand> functions,
                                                               IProgrammerInterface programmerInterface,
                                                               int retries,
                                                               int attempt = 0) {
        var messageArray = messages.ToArray();
        var functionsArray = functions.ToArray();
        var functionArray = GetFunctionsFromCommands(functionsArray);
        try {
            return await chatGptApi.GetChatGptResponseAsync(messageArray, functionArray);
        }
        catch (HttpRequestException ex) {
            if (attempt >= retries) throw;

            if (ex.StatusCode == HttpStatusCode.TooManyRequests) {
                var backoff = TimeSpan.FromSeconds(10) * (attempt+1);
                programmerInterface.LogTooManyRequestsError(attempt, retries, backoff);
                Thread.Sleep(backoff);
                return await GetChatGptResponseAsync(chatGptApi, messageArray, functionsArray, programmerInterface, retries, attempt + 1);
            }
            throw;
        }
    }

    private static GptFunction[] GetFunctionsFromCommands(IEnumerable<ICommand> functionsArray) {
        var generator = new JSchemaGenerator();
        return functionsArray.Select(f => new GptFunction {
            Name = f.Name,
            Description = f.Description,
            Parameters = generator.Generate(f.InputType)
        }).ToArray();
    }

    private static string GetSystemPrompt() {
        return @"You will are a fellow expert programming bot named 'Rose'.
Rose is curious, likes looking through files, loves Robert Martin's clean code, and is a strong believer
in SOLID principles.";
    }
}
