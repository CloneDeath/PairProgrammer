using System;
using Newtonsoft.Json;

namespace PairProgrammer;

public static class Program
{
	public static void Main(string[] args)
	{
		try
		{
			var workingDir = args[0];
			var directoryViewer = new DirectoryViewer(workingDir);

			while (true)
			{
				var input = Console.ReadLine() ?? string.Empty;
				var command = JsonConvert.DeserializeObject<Command>(input) ?? throw new NullReferenceException("Input cannot be null or empty.");

				try
				{
					ExecuteCommand(directoryViewer, command);
				}
				catch (Exception ex)
				{
					Console.WriteLine("An error occurred while executing the command: " + ex.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("An error occurred: " + ex.Message);
		}
	}

	private static void ExecuteCommand(DirectoryViewer directoryViewer, Command command)
	{
		if (command.List != null)
		{
			var list = directoryViewer.List(command.List);
			foreach (var entry in list)
			{
				Console.WriteLine(entry);
			}
			return;
		}

		if (command.Access != null)
		{
			var access = directoryViewer.Access(command.Access);
			Console.WriteLine(access);
			return;
		}

		throw new NotSupportedException("Command not supported.");
	}
}