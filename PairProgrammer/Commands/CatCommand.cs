using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands; 

public class CatCommand : ICommand {
	private readonly DirectoryViewer _directoryViewer;

	public CatCommand(DirectoryViewer directoryViewer) {
		_directoryViewer = directoryViewer;
	}

	public string Name => "cat";

	public string Execute(string[] args, string input) {
		var pattern = args.Length > 0 ? args[0] : string.Empty;
		if (_directoryViewer.IsDirectory(pattern)) {
			return $"cat: {pattern}: Is a directory";
		}

		var files = GetFiles(pattern).ToArray();
		if (!files.Any()) return $"cat: {pattern}: No such file or directory";

		var contents = files.Select(f => _directoryViewer.Access(f));
		return string.Join(Environment.NewLine, contents);
	}

	public IEnumerable<string> GetFiles(string pattern) {
		if (pattern.Contains('/')) throw new NotSupportedException();
		if (pattern.Contains("**")) throw new NotSupportedException();

		var files = _directoryViewer.ListFiles(".")
									.Select(f => _directoryViewer.GetLocalPath(f));
		var regex = GlobToRegex.Convert(pattern);
		return files.Where(f => regex.IsMatch(f));
	}
}