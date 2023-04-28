using System.Collections.Generic;
using System.Linq;

namespace PairProgrammer; 

public class ArgumentChain {
	private readonly List<string> _args;
	public ArgumentChain(IEnumerable<string> args) {
		_args = args.ToList();
	}

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
}