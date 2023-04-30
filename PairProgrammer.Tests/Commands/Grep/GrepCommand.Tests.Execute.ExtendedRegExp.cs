using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep;

[TestFixture]
public class GrepCommand_Tests_Execute_ExtendedRegExp {
	[Test]
	public void IfUsingExtendedRegExp_ThenParenthesisAreNotSwapped() {
		var input = "var x = DoThing('hello', 'world') + string.empty;";
		var command = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = command.Execute(new[] { "--extended-regexp", "DoThing(.*)" }, input);

		output.Should().Be("DoThing('hello', 'world') + string.empty;");
	}
}