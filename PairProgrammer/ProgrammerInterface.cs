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

	public virtual void LogList(string path) {
		Console.WriteLine($"AI: `ls {path}`");
	}

	public virtual void LogAccess(string filePath) {
		Console.WriteLine($"AI: `cat {filePath}`");
	}
}