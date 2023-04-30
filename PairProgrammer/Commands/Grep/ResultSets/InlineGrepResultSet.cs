using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands.Grep.ResultSets; 

public class InlineGrepResultSet : IGrepResultSet {
	private readonly Regex _regex;
	private readonly int? _maxCount;
	private readonly List<string> _results = new();
	private int _count;

	public string Context => string.Empty;

	public InlineGrepResultSet(Regex regex, int? maxCount) {
		_regex = regex;
		_maxCount = maxCount;
	}

	public virtual void Push(string line) {
		if (_count >= _maxCount) return;
		if (_regex.IsMatch(line)) {
			_results.Add(line);
			_count++;
		}
	}

	public virtual string GetOutput() {
		return string.Join(Environment.NewLine, _results);
	}

	public string GetCount() => _results.Count.ToString();
}