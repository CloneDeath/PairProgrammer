namespace PairProgrammer.Commands; 

public class CatCommand : ICommand {
	private readonly DirectoryViewer _directoryViewer;

	public CatCommand(DirectoryViewer directoryViewer) {
		_directoryViewer = directoryViewer;
	}

	public string Name => "cat";

	public string Execute(string[] args, string input) {
		var path = args.Length > 0 ? args[0] : string.Empty;
		if (_directoryViewer.IsDirectory(path)) {
			return $"cat: {path}: Is a directory";
		}
		return _directoryViewer.Exists(path) 
				   ? _directoryViewer.Access(path) 
				   : $"cat: {path}: No such file or directory";
	}
}