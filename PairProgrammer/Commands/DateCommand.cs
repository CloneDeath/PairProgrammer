using System;

namespace PairProgrammer.Commands; 

public class DateCommand : ICommand {
	public string Name => "date";

	public string Execute(string[] args, string input) {
		if (args.Length == 0) {
			return DateTime.Now.ToString("ddd MMM dd hh:mm:ss tt zz yyyy");
		}
		if (args.Length > 1) {
			return $"date: extra operand '{args[1]}'";
		}

		var csFormat = args[0].Replace("'", "")
							  .Replace("+", "")
							  .Replace("%r", "hh:mm:ss tt")
							  .Replace("%A", "dddd")
							  .Replace("%B", "MMMM")
							  .Replace("%d", "dd")
							  .Replace("%Y", "yyyy");
		return DateTime.Now.ToString(csFormat);
	}
}