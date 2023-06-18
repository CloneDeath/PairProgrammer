using System;
using PairProgrammer.GptApi;

namespace PairProgrammer.Tests; 

public class MockProgrammerInterface : IProgrammerInterface {
	public virtual string GetMessage() => string.Empty;
	public virtual void LogException(string responseText, Exception ex) { }
	public void LogTooManyRequestsError(int attempt, int retries, TimeSpan backoff) { }
	public void LogAiMessage(string content) { }
	public void LogFunctionCall(FunctionCall functionCall) { }
	public void LogFunctionResult(object result) { }
}