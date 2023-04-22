using System;

namespace PairProgrammer.Tests; 

public class MockProgrammerInterface : IProgrammerInterface{
	public virtual string GetMessage() => string.Empty;

	public virtual void LogCommand(Command command) { }

	public virtual void LogInvalidCommand(CommandNotRecognizedException ex) { }

	public virtual void LogException(string responseText, Exception ex) { }
}