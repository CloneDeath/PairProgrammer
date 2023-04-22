using System;

namespace PairProgrammer; 

public class ProgrammerInterface {
	public virtual string GetMessage() {
		Console.Write("[User]: ");
		return Console.ReadLine() ?? string.Empty;
	}

	public virtual void LogCommand(Command command) {
		if (!string.IsNullOrEmpty(command.Chat)) {
			Console.WriteLine($"[AI]: {command.Chat}");
		}
		if (!string.IsNullOrEmpty(command.Bash)) {
			Console.WriteLine($"[AI]: {command.Comment}");
			Console.WriteLine($"> {command.Bash}");
		}
	}

	public virtual void LogInvalidCommand(CommandNotRecognizedException ex) {
		Console.WriteLine($"AI tried to use invalid command `{ex.CommandType}`");
	}

	public virtual void LogException(string responseText, Exception ex) {
		Console.WriteLine($"[AI]: `{responseText}`");
		Console.WriteLine($"An error occurred while executing the command: {ex}`.");
	}
}