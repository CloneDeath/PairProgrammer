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
		var doCount = args.Contains("-c") || args.Contains("--count");
		var recursive = args.Contains("-r") || args.Contains("--recursive");
		var maxCount = args.Contains("-m") || args.Contains("--max-count") ? 0 : 1;
		var afterContext = args.Contains("-A") || args.Contains("--after-context");
		var beforeContext = args.Contains("-B") || args.Contains("--before-context");

		var nonFlags = args.Where(a => !a.StartsWith("-")).ToArray();
		if (nonFlags.Length == 0) return "Usage: grep [OPTION]... PATTERNS [FILE]...";

		var pattern = nonFlags[0];
		if (string.IsNullOrEmpty(pattern)) return "Invalid usage of 'grep' command. Please try again.";
		var regex = new Regex(pattern);

		var scope = nonFlags.Length > 1 ? nonFlags[1] : string.Empty;
        
		if (doCount) {
			var count = input.Split(Environment.NewLine).Count(line => regex.IsMatch(line));
			return count.ToString();
		}
		if (recursive) {
			var results = new List<string>();
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
			return string.Join(Environment.NewLine, results);
		}
		return "Invalid usage of 'grep' command. Please try again.";
	}
}