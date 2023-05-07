using System.Collections.Generic;
using System.Text;

namespace PairProgrammer; 

public static class CommandSplitter {
	public static IEnumerable<string> Split(string bash)
	{
		var result = new List<string>();
		var currentCommand = new StringBuilder();
		var inSingleQuote = false;
		var inDoubleQuote = false;

		foreach (var c in bash) {
			switch (c) {
				case '\'' when !inDoubleQuote:
					inSingleQuote = !inSingleQuote;
					break;
				case '\"' when !inSingleQuote:
					inDoubleQuote = !inDoubleQuote;
					break;
			}

			if (c == '|' && !inSingleQuote && !inDoubleQuote)
			{
				result.Add(currentCommand.ToString().Trim());
				currentCommand.Clear();
			}
			else
			{
				currentCommand.Append(c);
			}
		}

		if (currentCommand.Length > 0)
		{
			result.Add(currentCommand.ToString().Trim());
		}

		return result;
	}
}