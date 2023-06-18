using System;

namespace PairProgrammer;

public interface IProgrammerInterface {
	string GetMessage();
	void LogException(string responseText, Exception ex);
	void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff);
	void LogAiMessage(string content);
}

public class ProgrammerInterface : IProgrammerInterface {
	public virtual string GetMessage() {
		Console.Write("[User]: ");
		return Console.ReadLine() ?? string.Empty;
	}

	public virtual void LogException(string responseText, Exception ex) {
		Console.WriteLine($"[Rose]: `{responseText}`");
		Console.WriteLine($"An error occurred while executing the command: {ex}`.");
	}

	public virtual void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff) {
		var seconds = backoff.TotalSeconds;
		Console.WriteLine($"Loading (too many requests, retrying in {seconds} seconds)... {attempt}/{retries} retries left...");
	}

	public void LogAiMessage(string content) {
		Console.WriteLine($"[Rose]: {content}");
	}
}