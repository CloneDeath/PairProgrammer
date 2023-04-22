using System;
using System.Collections.Generic;
using System.IO.Abstractions;
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
        var programmerInterface = new ProgrammerInterface();
        var commandExecutor = new CommandExecutor(args[0], programmerInterface, new FileSystem());

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
                              ?? throw new NullReferenceException();
                var output = commandExecutor.ExecuteCommand(command);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = $"[User] {output}"
                });
            }
            catch (CommandNotRecognizedException ex) {
                programmerInterface.LogInvalidCommand(ex);
                var exResponse = new {
                    output = $"Command '{ex.CommandType}' not found." + Environment.NewLine
                                                                      + "(remember to use the `chat` JSON to communicate with the user)"
                };
                messages.Add(new Message {
                    Role = Role.User,
                    Content = JsonConvert.SerializeObject(exResponse)
                });
            }
            catch (Exception ex) {
                programmerInterface.LogException(responseText, ex);
                var exResponse = new {
                    user = programmerInterface.GetMessage()
                };
                messages.Add(new Message {
                    Role = Role.User,
                    Content = JsonConvert.SerializeObject(exResponse)
                });
            }
        }
    }

    private static string GetPrompt(string input) {
        return @"You will act as a fellow expert programming bot named 'Rose'
Rose only responds with one of two JSON objects:
1) {""chat"": ""Your message here""}
2) {""bash"": ""<bash command here>"", ""comment"": ""comment explaining command here""}

Rose can use the bash json to run Linux CLI commands on the user's terminal, to get information about their project.
Some commands Rose can run include: ls, cat, grep, wc

An example conversation with Rose may be:
{""user"": ""How are you today?""}
{""chat"": ""I am good, how are you?""}
{""user"": ""I'm doing fine. I was wondering, how many .cs files do I have in my project?""}
{""bash"": ""ls -R | grep -c \"".cs$\"", ""comment"": ""Listing files recursively, and using grep to count the number of .cs files.""}
{""output"", ""12""}
{""chat"": ""You have 12 .cs files in your project.""}
{""chat"": ""Does that include subdirectories?""}
{""chat"": ""Yes""}

Another example may be:
{""user"": ""Can you summerize this project?""}
{""bash"": ""cat README.md"", ""comment"": ""Outputting the contents of the README.md file, which should contain a summary of the project.""}
{""output"": ""cat: README.md: No such file or directory""}
{""chat"": ""I'm sorry, it seems that the README.md file doesn't exist in this project.\nIs there anything else I can help you with?""}

Please stay in character as Rose at all times. Always respond as Rose would, using those 2 JSON responses.

Now, please process the following user instructions, as Rose: "
               + Environment.NewLine + input;
    }
}
