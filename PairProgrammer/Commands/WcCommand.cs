using System;

namespace PairProgrammer.Commands; 

public class WcCommand : ICommand {
	public string Name => "wc";

	public string Execute(string[] args, string input) {
		var flag = args.Length > 0 ? args[0] : string.Empty;
		if (flag == "-l") {
			var count = input.Split(Environment.NewLine).Length;
			return count.ToString();
		}
		return "Invalid usage of 'wc' command. Please try again.";
	}
}