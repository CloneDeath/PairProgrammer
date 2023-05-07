using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PairProgrammer; 

public static class ArgumentSplitter
{
	
	public static string[] Split(string arguments)
	{
		if (string.IsNullOrEmpty(arguments))
		{
			return Array.Empty<string>();
		}

		const string pattern = @"(?:\s*(?:""(?<arg>[^""]*)""|'(?<arg>[^']*)'|(?<arg>[^\s]+))\s*)";
		var regex = new Regex(pattern);
		var matches = regex.Matches(arguments);
		var result = new List<string>();

		foreach (Match match in matches)
		{
			result.Add(match.Groups["arg"].Value);
		}

		return result.ToArray();
	}
}