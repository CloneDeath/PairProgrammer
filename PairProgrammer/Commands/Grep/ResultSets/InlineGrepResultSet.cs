using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer.Commands.Grep.ResultSets; 

public class InlineGrepResultSet : IGrepResultSet {
	private readonly Regex _regex;
	private readonly int? _maxCount;
	private int _count;
	private FixedSizeQueue<string> _afterQueue;
	private bool _needsDumping;
	private readonly List<string> _results = new();

	public string Context => string.Empty;

	public InlineGrepResultSet(Regex regex, int? maxCount, int afterContext) {
		_regex = regex;
		_maxCount = maxCount;
		_afterQueue = new FixedSizeQueue<string>(afterContext);
	}

	public virtual void Push(string line) {
		if (_regex.IsMatch(line)) {
			if (_count >= _maxCount) return;
			if (_needsDumping) {
				_results.Add("--");
			}
			_results.Add(line);
			_count++;
			_afterQueue.Clear();
			return;
		}

		if (!_results.Any()) return;
		_needsDumping = !_afterQueue.TryPushBack(line);
		if (!_needsDumping) {
			_results.Add(line);
		}
	}

	public virtual string GetOutput() {
		return string.Join(Environment.NewLine, _results);
	}

	public string GetCount() => _results.Count.ToString();
}