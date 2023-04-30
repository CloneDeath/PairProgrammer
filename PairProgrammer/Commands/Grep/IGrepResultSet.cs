namespace PairProgrammer.Commands.Grep; 

public interface IGrepResultSet {
	string Context { get; }
	void Push(string line);
	string GetOutput();
	string GetCount();
}