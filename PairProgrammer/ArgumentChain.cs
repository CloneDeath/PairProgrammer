using System;
using System.Collections.Generic;
using System.Linq;

namespace PairProgrammer; 

public class ArgumentChain {
	private readonly List<string> _args;
	public ArgumentChain(IEnumerable<string> args) {
		_args = args.ToList();
	}

	public IEnumerable<string> Arguments => _args;

	public bool SliceFlag(params string[] flags) {
		var result = false;
		foreach (var flag in flags) {
			result |= SliceSingleFlag(flag);
		}
		return result;
	}

	private bool SliceSingleFlag(string flag) {
		if (!_args.Contains(flag)) return false;
		_args.Remove(flag);
		return true;
	}

	public int? SliceInteger(params string[] arguments) {
		var value = TrySliceString(arguments, out var x) ? x : null;
		return value == null ? null : Convert.ToInt32(value);
	}

	public string? SliceString(params string[] arguments) {
		return TrySliceString(arguments, out var x) ? x : null;
	}

	private bool TrySliceString(string[] arguments, out string value) {
		for (var i = 0; i < _args.Count; i++) {
			var arg = _args[i];

			foreach (var argument in arguments) {
				if (!arg.StartsWith(argument)) continue;

				if (arg.StartsWith($"{argument}=")) {
					value = arg.Replace($"{argument}=", string.Empty);
					_args.RemoveAt(i);
					return true;
				}

				value = _args[i + 1];
				_args.RemoveAt(i+1);
				_args.RemoveAt(i);
				return true;
			}
		}

		value = string.Empty;
		return false;
	}
}