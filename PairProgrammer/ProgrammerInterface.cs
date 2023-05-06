using System;

namespace PairProgrammer;

public interface IProgrammerInterface {
	string GetMessage();
	void LogCommand(Command command);
	void LogInvalidCommand(CommandNotRecognizedException ex);
	void LogException(string responseText, Exception ex);
}

public class ProgrammerInterface : IProgrammerInterface {
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

	public virtual void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff) {
		var seconds = backoff.TotalSeconds;
		Console.WriteLine($"Loading (too many requests, retrying in {seconds} seconds)... {attempt}/{retries} retries left...");
	}
}