namespace PairProgrammer; 

public interface ICommand {
	public string Name { get; }
	public string Execute(string[] args, string input);
}