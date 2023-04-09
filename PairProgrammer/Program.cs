using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PairProgrammer;

public static class Program
{
	public static async Task Main(string[] args)
	{
		string apiKey = "your-api-key";

		var chatGptApi = new ChatGptApi(apiKey);
		var commandExecutor = new CommandExecutor(args[0]);

		while (true)
		{
			Console.WriteLine("Enter your message:");
			string input = Console.ReadLine();

			var prompt = "As an AI language model, you will help in pair programming for a software project. "
						 + "You will be given commands in JSON format like `{\"access\": \"filename.txt\"}` to access a file, "
						 + "or `{\"list\":\"/\"}` to list all files in a directory. "
						 + "You will use your knowledge to provide useful responses and assistance in software development. "
						 + "Now, please process the following user input: "
						 + Environment.NewLine + input;
			var response = await chatGptApi.GetChatGptResponseAsync(prompt);

			try
			{
				var command = JsonConvert.DeserializeObject<Command>(response) ?? throw new NullReferenceException("Input cannot be null or empty.");
				string output = commandExecutor.ExecuteCommand(command);
				Console.WriteLine(output);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred while executing the command: " + ex.Message);
			}
		}
	}
}