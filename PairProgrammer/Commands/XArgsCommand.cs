using System.Collections.Generic;
using System.Linq;

namespace PairProgrammer.Commands; 

public class XArgsCommand : ICommand {
	private readonly ICommandFactory _factory;
	public string Name => "xargs";

	public XArgsCommand(ICommandFactory factory) {
		_factory = factory;
	}
	public string Execute(string[] args, string input) {
		var commandType = args.First();

		var command = _factory.GetCommand(commandType);
		var arguments = ArgumentSplitter.Split(input);

		var results = new List<string>();
		foreach (var argument in arguments) {
			var result = command.Execute(new[]{argument}, string.Empty);
			results.Add(result);
		}
		return string.Join(string.Empty, results);
	}
}