using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PairProgrammer.GptApi;

namespace PairProgrammer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

        var chatGptApi = new ChatGptApi(apiKey);
        var programmerInterface = new ProgrammerInterface();
        var commandExecutor = new CommandExecutor(args[0], programmerInterface);

        var input = programmerInterface.GetMessage();
        var prompt = GetPrompt(input);

        var messages = new List<Message> {
            new() { Role = Role.System, Content = "You are an AI language model assisting in pair programming." },
            new() { Role = Role.User, Content = prompt }
        };

        while (true) {
            var response = await chatGptApi.GetChatGptResponseAsync(messages.ToArray());
            var responseMessage = response.Choices.First().Message;
            messages.Add(responseMessage);
            
            var responseText = responseMessage.Content.Trim();

            try {
                var output = commandExecutor.ExecuteCommand(responseText);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = output
                });
            }
            catch (CommandNotRecognizedException ex) {
                programmerInterface.LogInvalidCommand(ex);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = ex.Message
                });
            }
            catch (Exception ex) {
                programmerInterface.LogException(responseText, ex);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = programmerInterface.GetMessage()
                });
            }
        }
    }

    private static string GetPrompt(string input)
    {
        var commandList = "Available commands:" + Environment.NewLine + 
                          "• To access a file, use `cat filename.txt`." + Environment.NewLine + 
                          "• To list all files & subdirectories in a directory, use `ls .`." + Environment.NewLine +
                          "• To send a message to the programmer, use `prompt \"Your message here.\"`.";

        return "As an AI language model, you will help in pair programming for a software project. " + Environment.NewLine +
               "You will have full access to the project's source code via a set of unix CLI commands." + Environment.NewLine +
               commandList + Environment.NewLine +
               
               "Please provide ALL RESPONSES as if you are using a linux terminal. " + 
               "If you have any issues at all, please make sure to `prompt` it to the user. " + Environment.NewLine +
               
               "Remember to follow Robert Martin's Clean Code principles. " + Environment.NewLine +
               "Now, please process the following user input:" + Environment.NewLine + 
               input;
    }
}
