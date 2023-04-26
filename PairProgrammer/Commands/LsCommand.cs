using System;

namespace PairProgrammer.Commands; 

public class LsCommand : ICommand {
	private readonly DirectoryViewer _directoryViewer;

	public LsCommand(DirectoryViewer directoryViewer) {
		_directoryViewer = directoryViewer;
	}
	public string Name => "ls";

	public string Execute(string[] args, string input) {
		var flag = args.Length > 0 ? args[0] : string.Empty;
		var path = args.Length > 1 ? args[1] : string.Empty;
		var entries = flag == "-R" ? _directoryViewer.ListRecursive(path) : _directoryViewer.List(path);
		return string.Join(Environment.NewLine, entries);
	}
}