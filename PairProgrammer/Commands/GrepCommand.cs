using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands; 

public class GrepCommand : ICommand {
	private readonly DirectoryViewer _directoryViewer;

	public GrepCommand(DirectoryViewer directoryViewer) {
		_directoryViewer = directoryViewer;
	}
	
	public string Name => "grep";

	public string Execute(string[] args, string input) {
		var chain = new ArgumentChain(args);
		var doCount = chain.SliceFlag("-c", "--count");
		var recursive = chain.SliceFlag("-r", "--recursive");
		var maxCount = chain.SliceInteger("-m", "--max-count");
		var afterContext = chain.SliceInteger("-A", "--after-context");
		var beforeContext = chain.SliceInteger("-B", "--before-context");

		var remainingArgs = chain.Arguments.ToArray();
		foreach (var remainingArg in remainingArgs) {
			if (remainingArg.StartsWith("-")) throw new Exception($"Unrecognized argument {remainingArg}");
		}
		if (remainingArgs.Length == 0) return "Usage: grep [OPTION]... PATTERNS [FILE]...";

		var pattern = remainingArgs[0];
		if (string.IsNullOrEmpty(pattern)) return "Invalid usage of 'grep' command. Please try again.";
		var regex = new Regex(pattern);

		var scope = remainingArgs.Length > 1 ? remainingArgs[1] : null;

		var results = new List<string>();
		if (scope == null) {
			var lines = input.Split(Environment.NewLine);
			foreach (var line in lines) {
				if (regex.IsMatch(line)) results.Add(line);
			}
		} else if (recursive) {
			var files = _directoryViewer.ListRecursive(scope);
			foreach (var file in files) {
				var localFile = _directoryViewer.GetLocalPath(file);
				var fileText = _directoryViewer.Access(file);
				var fileLines = fileText.Split(Environment.NewLine);
				foreach (var fileLine in fileLines) {
					var matches = regex.Matches(fileLine);
					foreach (Match match in matches) {
						results.Add($"{localFile}: {fileLine}");
					}
				}
			}
		}

		results = maxCount != null ? results.Take((int)maxCount).ToList() : results;
		if (doCount) return results.Count.ToString();
		return results.Any() 
				   ? string.Join(Environment.NewLine, results)
				   : "Invalid usage of 'grep' command. Please try again.";
	}
}