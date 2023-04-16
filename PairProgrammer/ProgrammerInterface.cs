using System;

namespace PairProgrammer; 

public class ProgrammerInterface {
	public virtual string GetMessage() {
		Console.Write("Programmer: ");
		return Console.ReadLine() ?? string.Empty;
	}
}