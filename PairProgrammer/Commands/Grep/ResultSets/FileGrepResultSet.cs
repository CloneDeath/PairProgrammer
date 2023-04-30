using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands.Grep.ResultSets; 

public class FileGrepResultSet : IGrepResultSet {
	private readonly Regex _regex;
	private readonly int? _maxCount;
	private readonly List<string> _results = new();

	public string Context { get; }

	public FileGrepResultSet(string localFile, Regex regex, int? maxCount) {
		Context = localFile;
		_regex = regex;
		_maxCount = maxCount;
	}

	public void Push(string line) {
		var matches = _regex.Matches(line);
		foreach (Match _ in matches) {
			_results.Add(line);
		}
	}

	public string GetOutput() {
		var lines = _results.Select(l => $"{Context}: {l}");
		return string.Join(Environment.NewLine, lines);
	}

	public string GetCount() {
		return $"{Context}: {_results.Count}";
	}
}