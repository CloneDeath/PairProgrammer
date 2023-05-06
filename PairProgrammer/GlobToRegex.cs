using System.Text.RegularExpressions;

namespace PairProgrammer; 

public static class GlobToRegex {
	public static Regex Convert(string glob) {
		var regexPattern = glob.Replace(".", "\\.")
							   .Replace("$", "\\$")
							   .Replace("*", "[^\\/]+");
		var prefix = @"^(\./|/)?";
		return new Regex(prefix + regexPattern + "$");
	}
}