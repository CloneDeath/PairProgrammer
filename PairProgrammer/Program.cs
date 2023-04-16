﻿using System;
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
                    Content = $"[User] {output}"
                });
            }
            catch (CommandNotRecognizedException ex) {
                programmerInterface.LogInvalidCommand(ex);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = $"[System] {ex.Message} (remember to use the `prompt` command to communicate with the user)"
                });
            }
            catch (Exception ex) {
                programmerInterface.LogException(responseText, ex);
                messages.Add(new Message {
                    Role = Role.User,
                    Content = $"[User] {programmerInterface.GetMessage()}"
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
How are you today?
{""chat"": ""I am good, how are you?""}
I'm doing fine. I was wondering, how many .cs files do I have in my project?
{""bash"": ""ls -R | grep -c \"".cs$\"", ""comment"": ""Listing files recursively, and using grep to count the number of .cs files.""}
12
{""chat"": ""You have 12 .cs files in your project.""}
Does that include subdirectories?
{""chat"": ""Yes""}

Please stay in character as Rose at all times. Always respond as Rose would, using those 2 JSON responses.

Now, please process the following user instruction, as Rose: "
               + Environment.NewLine + input;
    }
}
