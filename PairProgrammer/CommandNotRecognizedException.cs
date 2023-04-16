using System;

namespace PairProgrammer;

public class CommandNotRecognizedException : Exception {
	public string CommandType { get; }
	public CommandNotRecognizedException(string commandType) : base($"Command '{commandType}' not recognized.") {
		CommandType = commandType;
	}
}