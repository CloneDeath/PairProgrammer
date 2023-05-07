using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands.Grep; 

public class GrepCommand : ICommand {
	private readonly DirectoryViewer _directoryViewer;

	public GrepCommand(DirectoryViewer directoryViewer) {
		_directoryViewer = directoryViewer;
	}
	
	public string Name => "grep";

	public string Execute(string[] args, string input)
	{
	    var chain = new ArgumentChain(args);
	    var doCount = chain.SliceFlag("-c", "--count");
	    var recursive = chain.SliceFlag("-r", "--recursive", "-R", "--dereference-recursive");
	    var maxCount = chain.SliceInteger("-m", "--max-count");
	    var ignoreCase = chain.SliceFlag("-i", "--ignore-case");
	    var extendedRegExp = chain.SliceFlag("-E", "--extended-regexp");
	    var afterContext = chain.SliceInteger("-A", "--after-context") ?? 0;
	    var include = chain.SliceString("--include");
	    var beforeContext = chain.SliceInteger("-B", "--before-context") ?? 0;

	    var remainingArgs = chain.Arguments.ToArray();
	    foreach (var remainingArg in remainingArgs)
	    {
	        if (remainingArg.StartsWith("-")) throw new Exception($"Unrecognized argument {remainingArg}");
	    }
	    if (remainingArgs.Length == 0) return "Usage: grep [OPTION]... PATTERNS [FILE]...";

	    var pattern = remainingArgs[0];
	    if (string.IsNullOrEmpty(pattern)) return "Invalid usage of 'grep' command. Please try again.";
	    if (!extendedRegExp) pattern = SwapRegexParenthesis(pattern);
	    var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
	    var regex = new Regex(pattern, options);

	    var scope = remainingArgs.Length > 1 ? remainingArgs[1] : null;

	    var results = new GrepResultSetCollection(regex, maxCount, afterContext);
	    if (scope == null && !recursive)
	    {
	        var lines = input.Split(Environment.NewLine);
	        foreach (var line in lines)
	        {
	            results.Push(line);
	        }
	    }
	    else
	    {
	        var files = GetFiles(scope, recursive);
	        if (!string.IsNullOrEmpty(include))
	        {
	            files = FilterFilesByIncludePattern(files, include);
	        }
	        foreach (var file in files)
	        {
	            var localFile = _directoryViewer.GetLocalPath(file);

	            var fileText = _directoryViewer.Access(file);
	            var fileLines = fileText.Split(Environment.NewLine);
	            foreach (var fileLine in fileLines)
	            {
	                results.Push(localFile, fileLine);
	            }
	        }
	    }

	    return doCount ? results.GetCount() : results.GetOutput();
	}

	public IEnumerable<string> FilterFilesByIncludePattern(IEnumerable<string> files, string includePattern)
	{
	    var includeRegex = GlobToRegex.Convert(includePattern);
	    return files.Where(f => includeRegex.IsMatch(_directoryViewer.GetFileName(f)));
	}


	public IEnumerable<string> GetFiles(string? scope, bool recursive) {
		if (scope == null) {
			if (recursive) {
				return _directoryViewer.ListFilesRecursive(".");
			}
			throw new NotSupportedException();
		}
		if (_directoryViewer.IsFile(scope)) {
			return new[]{scope};
		}
		if (_directoryViewer.IsDirectory(scope)) {
			return _directoryViewer.ListFilesRecursive(scope);
		}

		var fileRegex = GlobToRegex.Convert(scope);
		var allFiles = _directoryViewer.ListFilesRecursive(".");
		return allFiles.Where(f => fileRegex.IsMatch(_directoryViewer.GetLocalPath(f)));
	}
	
	public static string SwapRegexParenthesis(string pattern) {
		return Regex.Replace(pattern, @"\\?[\(\)]", m => ParenthesisSwapEvaluator(m.Value));
	}

	public static string ParenthesisSwapEvaluator(string value) {
		if (value.StartsWith("\\")) {
			return value.EndsWith(")") ? ")" : "(";
		}
		return value == "(" ? "\\(" : "\\)";
	}
}