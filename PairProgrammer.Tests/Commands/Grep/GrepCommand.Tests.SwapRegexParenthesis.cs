using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep; 

[TestFixture]
public class GrepCommand_Tests_SwapRegexParenthesis : GrepCommand_Tests {
	[TestCase("()", "\\(\\)")]
	[TestCase("\\(\\)", "()")]
	[TestCase("\\(\\)()", "()\\(\\)")]
	[TestCase("()\\(\\)", "\\(\\)()")]
	[TestCase("(\\)\\()", "\\()(\\)")]
	[TestCase("\\()(\\)", "(\\)\\()")]
	public void ItShouldSwapAllParenthesis(string input, string expected) {
		var output = GrepCommand.SwapRegexParenthesis(input);

		output.Should().Be(expected);
	}
}