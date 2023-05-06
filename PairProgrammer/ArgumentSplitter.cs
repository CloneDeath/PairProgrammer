using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer; 

public static class ArgumentSplitter {
	public static string[] Split(string arguments) {
		return Regex.Matches(arguments, @"[\+\w]*([^\s""']+|""([^""]*)""|'([^']*)')")
					.Select(m => m.Value.Trim('\'', '\"'))
					.Where(s => !string.IsNullOrEmpty(s))
					.ToArray();
	}
}