using System;
using System.Linq;
using System.Text.Json;
using PairProgrammer.GptApi;

namespace PairProgrammer;

public interface IProgrammerInterface {
	string GetMessage();
	void LogException(string responseText, Exception ex);
	void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff);
	void LogAiMessage(string content);
	void LogFunctionCall(FunctionCall functionCall);
	void LogFunctionResult(object result);
	bool GetApprovalToWriteToFile(string file, string content);
}

public class ProgrammerInterface : IProgrammerInterface {
	private readonly ConsoleColor _defaultForeground;

	public ProgrammerInterface() {
		_defaultForeground = Console.ForegroundColor;
	}
	
	public virtual string GetMessage() {
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write("[User]: ");
		Console.ForegroundColor = _defaultForeground;
		return Console.ReadLine() ?? string.Empty;
	}

	public virtual void LogException(string responseText, Exception ex) {
		Output("Rose", ConsoleColor.Magenta, responseText);
		
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"An error occurred while executing the command: {ex}`.");
		Console.ForegroundColor = _defaultForeground;
	}

	public virtual void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff) {
		var seconds = backoff.TotalSeconds;
		Console.WriteLine($"Loading (too many requests, retrying in {seconds} seconds)... {attempt}/{retries} retries left...");
	}

	public void LogAiMessage(string content) {
		Output("Rose", ConsoleColor.Magenta, content);
	}

	public void LogFunctionCall(FunctionCall functionCall) {
		Console.ForegroundColor = ConsoleColor.DarkMagenta;
		Console.Write("@Rose > ");
		Console.ForegroundColor = ConsoleColor.DarkGray;
		var function = $"{functionCall.Name}({functionCall.Arguments});";
		Console.WriteLine(function.Replace(Environment.NewLine, string.Empty));
		Console.ForegroundColor = _defaultForeground;
	}

	public void LogFunctionResult(object result) {
		if (result is string stringResult) {
			var lines = stringResult.Split(Environment.NewLine);
			const int maxLines = 5;
			var outputLines = string.Join(Environment.NewLine, lines.Take(maxLines));
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(outputLines);
			if (lines.Length > maxLines) {
				var remaining = lines.Length - maxLines;
				Console.WriteLine($"... {remaining} lines omitted");
			}
			Console.ForegroundColor = _defaultForeground;
			return;
		}

		var content = JsonSerializer.Serialize(result);
		var outputContent = content.Substring(0, 100);
		if (outputContent.Length < content.Length) outputContent += "...";
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.WriteLine(outputContent);
		Console.ForegroundColor = _defaultForeground;
	}

	public bool GetApprovalToWriteToFile(string file, string content) {
		Output("System", ConsoleColor.Yellow, "Rose would like to write the following data: ");
		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine(file);
		Console.ForegroundColor = _defaultForeground;
		LogFunctionResult(content);
		Console.WriteLine();
		Output("System", ConsoleColor.Yellow, "Do you approve? [Y/n]");
		var response = Console.ReadKey(false);
		return response.Key is ConsoleKey.Y or ConsoleKey.Enter;
	}

	private void Output(string source, ConsoleColor color, string text) {
		Console.ForegroundColor = color;
		Console.Write($"[{source}]: ");
		Console.ForegroundColor = _defaultForeground;
		Console.WriteLine(text);
	}
}