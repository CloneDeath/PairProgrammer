using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep; 

[TestFixture]
public class GrepCommand_Tests_ParenthesisSwapEvaluator : GrepCommand_Tests {
	[TestCase(@"\\(", "(")]
	[TestCase(@"\\)", ")")]
	[TestCase(@")", "\\)")]
	[TestCase(@"(", "\\(")]
	public void ItSwapsEscapedAndUnescapedParens(string input, string expected) {
		var output = GrepCommand.ParenthesisSwapEvaluator(input);

		output.Should().Be(expected);
	}
}