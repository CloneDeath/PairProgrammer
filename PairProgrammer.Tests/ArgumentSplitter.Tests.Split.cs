using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class ArgumentSplitter_Tests_Split : ArgumentSplitter_Tests {
	[Test]
	public void ItIsAbleToSplitArguments() {
		const string bash = "echo hello world";

		var args = ArgumentSplitter.Split(bash);

		args.Should().HaveCount(3);
		args.Should().ContainInConsecutiveOrder("echo", "hello", "world");
	}
	
	[Test]
	public void ItIsAbleToGroupThingsUsingQuotes() {
		const string bash = "echo 'hello world'";

		var args = ArgumentSplitter.Split(bash);

		args.Should().HaveCount(2);
		args.Should().ContainInConsecutiveOrder("echo", "hello world");
	}
}