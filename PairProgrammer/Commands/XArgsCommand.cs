namespace PairProgrammer.Commands; 

public class XArgsCommand : ICommand {
	public string Name => "xargs";
	public string Execute(string[] args, string input) => throw new System.NotImplementedException();
}