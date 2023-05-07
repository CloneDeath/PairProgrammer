using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class CommandSplitter_Tests {
	[Test]
	public void ItShouldNotSplitIfThePipeIsPartOfACommandArgumentWrappedInAString() {
		const string bash = "grep -r -E 'class|namespace' --include='*.cs'";
		
		
		var commands = CommandSplitter.Split(bash).ToArray();

		commands.Should().HaveCount(1);
		commands.Should().ContainInConsecutiveOrder("grep -r -E 'class|namespace' --include='*.cs'");
	}

	[Test]
	public void ItShouldSplitTwoDifferentCommands_WhenTheyAreMeantToPipe() {
		const string bash = "cat hello.txt | grep 'class|namespace'";
		
		
		var commands = CommandSplitter.Split(bash).ToArray();

		commands.Should().HaveCount(2);
		commands.Should().ContainInConsecutiveOrder("cat hello.txt", "grep 'class|namespace'");
	}
}