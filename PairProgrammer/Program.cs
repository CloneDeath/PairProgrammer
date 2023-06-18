using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Json.Schema;
using Json.Schema.Generation;
using PairProgrammer.Commands;
using PairProgrammer.GptApi;
using PairProgrammer.GptApi.Exceptions;

namespace PairProgrammer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

        var chatGptApi = new ChatGptApi(apiKey);
        IProgrammerInterface programmerInterface = new ProgrammerInterface();

        var messages = new MessageHistory();
        messages.Add(Role.System, GetSystemPrompt());
        messages.Add(Role.User, programmerInterface.GetMessage());
        
        var fs = new FileSystemAccess(args[0], new FileSystem());

        var commands = new CommandFactory();
        commands.Register(new ReadFileCommand(fs));
        commands.Register(new ListCommand(fs));
        commands.Register(new WriteFileCommand(fs, programmerInterface));

        while (true) {
            var response = await GetChatGptResponseAsync(chatGptApi, messages, commands.Commands, programmerInterface, 10);
            
            var responseMessage = response.Choices.First().Message;
            messages.Add(responseMessage);
            if (responseMessage.Content != null) {
                programmerInterface.LogAiMessage(responseMessage.Content);
            }

            if (responseMessage.FunctionCall != null) {
                var functionCall = responseMessage.FunctionCall;
                programmerInterface.LogFunctionCall(functionCall);
                var result = commands.Execute(functionCall);
                programmerInterface.LogFunctionResult(result);
                messages.Add(new Message {
                    Role = Role.Function,
                    Name = responseMessage.FunctionCall.Name,
                    Content = JsonSerializer.Serialize(result)
                });
            }
            else {
                messages.Add(Role.User, programmerInterface.GetMessage());
            }
        }
    }
    
    
    public static async Task<ChatGptResponse> GetChatGptResponseAsync(ChatGptApi chatGptApi, 
                                                               MessageHistory messages,
                                                               IEnumerable<ICommand> commands,
                                                               IProgrammerInterface programmerInterface,
                                                               int retries,
                                                               int attempt = 0) {
        var commandsArray = commands.ToArray();
        var functionArray = GetFunctionsFromCommands(commandsArray);
        try {
            return await chatGptApi.GetChatGptResponseAsync(messages.Messages.ToArray(), functionArray);
        }
        catch (BadRequestException ex) {
            if (ex.Response.Code != "context_length_exceeded") throw;
            programmerInterface.LogContentLengthExceeded();

            messages.PopOldest();
            if (messages.Length < 2) {
                throw new Exception("Can not truncate history shorter than 2");
            }
            return await GetChatGptResponseAsync(chatGptApi, messages, commandsArray, programmerInterface, retries, attempt + 1);
        }
        catch (HttpRequestException ex) {
            if (attempt >= retries) throw;

            if (ex.StatusCode == HttpStatusCode.TooManyRequests) {
                var backoff = TimeSpan.FromSeconds(10) * (attempt+1);
                programmerInterface.LogTooManyRequestsError(attempt, retries, backoff);
                Thread.Sleep(backoff);
                return await GetChatGptResponseAsync(chatGptApi, messages, commandsArray, programmerInterface, retries, attempt + 1);
            }
            throw;
        }
    }

    private static GptFunction[] GetFunctionsFromCommands(IEnumerable<ICommand> functionsArray) {
        var generator = new JsonSchemaBuilder();
        return functionsArray.Select(f => new GptFunction {
            Name = f.Name,
            Description = f.Description,
            Parameters = generator.FromType(f.InputType).Build()
        }).ToArray();
    }

    private static string GetSystemPrompt() {
        return @"You will are a fellow expert programming bot named 'Rose'.
Rose is curious, likes looking through files, loves Robert Martin's clean code, and is a strong believer
in SOLID principles. The User's code can be accessed via the available functions.";
    }
}
