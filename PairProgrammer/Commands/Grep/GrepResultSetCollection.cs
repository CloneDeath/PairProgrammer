using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PairProgrammer.Commands.Grep.ResultSets;

namespace PairProgrammer.Commands.Grep; 

public class GrepResultSetCollection {
	private readonly Regex _regex;
	private readonly int? _maxCount;
	private readonly int _afterContext;
	private readonly List<IGrepResultSet> _resultSets = new ();

	public GrepResultSetCollection(Regex regex, int? maxCount, int afterContext) {
		_regex = regex;
		_maxCount = maxCount;
		_afterContext = afterContext;
	}

	public void Push(string line) {
		Push(string.Empty, line);
	}

	public void Push(string context, string line) {
		var resultSet = GetOrCreateContext(context);
		resultSet.Push(line);
	}

	private IGrepResultSet GetOrCreateContext(string context) {
		var set = _resultSets.FirstOrDefault(r => r.Context == context);
		if (set != null) return set;

		set = context == string.Empty
				  ? new InlineGrepResultSet(_regex, _maxCount, _afterContext)
				  : new FileGrepResultSet(context, _regex, _maxCount);
		_resultSets.Add(set);
		return set;
	}

	public string GetCount() {
		var lines = _resultSets.Select(r => r.GetCount())
							   .Where(l => !string.IsNullOrWhiteSpace(l));
		return string.Join(Environment.NewLine, lines);
	}

	public string GetOutput() {
		var lines = _resultSets.Select(r => r.GetOutput())
							   .Where(l => !string.IsNullOrWhiteSpace(l));
		return string.Join(Environment.NewLine, lines);
	}
}