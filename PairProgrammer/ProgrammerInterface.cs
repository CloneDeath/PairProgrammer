using System;
using System.Linq;
using Newtonsoft.Json;
using PairProgrammer.GptApi;

namespace PairProgrammer;

public interface IProgrammerInterface {
	string GetMessage();
	void LogException(string responseText, Exception ex);
	void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff);
	void LogAiMessage(string content);
	void LogFunctionCall(FunctionCall functionCall);
	void LogFunctionResult(object result);
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

	public void LogFunctionCall(FunctionCall functionCall) {
		Console.WriteLine($"@Rose > {functionCall.Name}({functionCall.Arguments});");
	}

	public void LogFunctionResult(object result) {
		if (result is string stringResult) {
			var lines = stringResult.Split(Environment.NewLine);
			const int maxLines = 5;
			var outputLines = string.Join(Environment.NewLine, lines.Take(maxLines));
			Console.WriteLine(outputLines);
			if (lines.Length > maxLines) {
				var remaining = lines.Length - maxLines;
				Console.WriteLine($"... {remaining} lines omitted");
			}
			return;
		}
		Console.WriteLine(JsonConvert.SerializeObject(result));
	}
}