using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PairProgrammer.GptApi;

namespace PairProgrammer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

        var chatGptApi = new ChatGptApi(apiKey);
        var commandExecutor = new CommandExecutor(args[0]);
        var programmerInterface = new ProgrammerInterface();
        
        
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
                var command = JsonConvert.DeserializeObject<Command>(responseText) 
                              ?? throw new NullReferenceException("Input cannot be null or empty.");
                var output = commandExecutor.ExecuteCommand(command);
                Console.WriteLine(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing the command: " + ex.Message);
            }
        }
    }

    private static string GetPrompt(string input)
    {
        var commandList = "Available commands:" + Environment.NewLine + 
                          "• To access a file, use `{\"access\": \"filename.txt\"}`." + Environment.NewLine + 
                          "• To list all files & subdirectories in a directory, use `{\"list\":\"./\"}`." + Environment.NewLine +
                          "• To send a message to the programmer, use `{\"message\": \"Your text here.\"}`.";

        return "As an AI language model, you will help in pair programming for a software project. " + Environment.NewLine +
               "You will have full access to the project's source code via a set of JSON commands." + Environment.NewLine +
               commandList + Environment.NewLine +
               
               "Please provide ALL RESPONSES inside of a JSON command as a response to handle the user input. " + 
               "ONLY valid JSON commands will be understood, and all other responses will be ignored. " + Environment.NewLine +
               
               "Remember to follow Robert Martin's Clean Code principles. " + Environment.NewLine +
               "Now, please process the following user input:" + Environment.NewLine + 
               input;
    }
}
