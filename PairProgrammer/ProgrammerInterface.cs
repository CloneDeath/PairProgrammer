using System;

namespace PairProgrammer; 

public class ProgrammerInterface {
	public virtual string GetMessage() {
		Console.Write("Programmer: ");
		return Console.ReadLine() ?? string.Empty;
	}

	public virtual void ShowMessage(string aiMessage) {
		Console.WriteLine($"AI: {aiMessage}");
	}
	
	public virtual void LogCommand(string command) {
		Console.WriteLine($"AI: `{command}`");
	}

	public virtual void LogInvalidCommand(CommandNotRecognizedException ex) {
		Console.WriteLine($"AI: tried to use invalid command `{ex.CommandType}`");
	}

	public virtual void LogException(string responseText, Exception ex) {
		Console.WriteLine($"AI: `{responseText}`");
		Console.WriteLine($"An error occurred while executing the command: {ex}`.");
	}
}