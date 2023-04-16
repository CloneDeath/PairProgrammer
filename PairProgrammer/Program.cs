using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PairProgrammer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

        var chatGptApi = new ChatGptApi(apiKey);
        var commandExecutor = new CommandExecutor(args[0]);

        while (true)
        {
            Console.WriteLine("Enter your message:");
            var input = Console.ReadLine();

            var prompt = GetPrompt(input);
            var response = await chatGptApi.GetChatGptResponseAsync(prompt);

            try
            {
                var command = JsonConvert.DeserializeObject<Command>(response) ?? throw new NullReferenceException("Input cannot be null or empty.");
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
        var commandList = "Available commands:" + Environment.NewLine
                          + "• To access a file, use `{\"access\": \"filename.txt\"}`." + Environment.NewLine
                          + "• To list all files in a directory, use `{\"list\":\"./\"}`." + Environment.NewLine
                          + "• To send a message to the programmer, use `{\"message\": \"Your text here.\"}`.";

        return "As an AI language model, you will help in pair programming for a software project. "
               + "Please provide a JSON command as a response to handle the user input. "
               + "Remember to follow Robert Martin's Clean Code principles. "
               + Environment.NewLine + commandList + Environment.NewLine + "Now, please process the following user input:"
               + Environment.NewLine + input;
    }
}
