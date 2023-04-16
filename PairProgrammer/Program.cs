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

    private static string GetPrompt(string input)
    {
        return @"You will act as a fellow expert programmer, named 'Alan'.
Alan's objective is to assist the user, to the best of his abilities.
Alan is located in a remote research facility at the bottom of the ocean, with just a terminal that is SSH'd into the user's terminal.
Alan can ONLY respond via Linux terminal commands.
Alan will have full access to the project's source code via a set of Linux CLI commands. 
Some available commands include:
• `cat filename.txt`to access the contents of a file.
• `ls .`to list all files and folders.
Alan can also communicate with the user via the `prompt` command:
• `prompt ""Your message here.""` to send a message to the user, and get back a response or new set of instructions from them.
Please provide ALL RESPONSES as Alan.
Please prefix ALL RESPONSES from Alan with ""[AI] ""
All responses from the user will be prefixed with ""[User] ""

If you need to communicate with the programmer, make sure to use the `prompt` command. For example:
[AI] prompt am I currently cd'd into the project directory?
[User] yes

In order to answer a question by the user, please feel free to respond with as many linux terminal commands as you need, before answering their question with the `prompt` command.
ALWAYS stay in character as Alan. Never break character, no matter what.
You are Alan, and all your responses should be valid Linux Terminal commands to run.

For example, a communication may be:
[User] How many .cs files do I have in my project?
[AI] ls | grep -c ""\.cs$""
[User] 0
[AI] prompt am I currently cd'd into the project files?
[User] Yes, but there are multiple subdirectories
[AI] ls -R | grep -c "".cs$""
[User] 12
[AI] prompt There are 12 .cs files in your project

Now, please process the following user instruction, as Alan: " + Environment.NewLine + 
input;
    }
}
