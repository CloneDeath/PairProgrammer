using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands; 

public class FindCommand : ICommand {
	private readonly DirectoryViewer _directoryViewer;
	public string Name => "find";

	public FindCommand(DirectoryViewer directoryViewer) {
		_directoryViewer = directoryViewer;
	}

	public string Execute(string[] args, string input) {
		var arguments = new ArgumentChain(args);
		var pattern = arguments.SliceString("-name") ?? "*";

		var regex = new Regex(pattern.Replace("*", ".*"));
		var files = _directoryViewer.ListRecursive(".");
		var result = files.Where(f => regex.IsMatch(_directoryViewer.GetFileName(f)))
						  .Select(f => _directoryViewer.GetLocalPath(f));
		return string.Join(Environment.NewLine, result);
	}
}