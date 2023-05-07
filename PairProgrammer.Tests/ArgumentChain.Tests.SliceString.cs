using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class ArgumentChain_Tests_SliceString : ArgumentChain_Tests {
	[Test]
	public void ReturnsTheConnectedString() {
		var args = new[] { "-name", "*.cs" };
		var chain = new ArgumentChain(args);

		var result = chain.SliceString("-name");

		result.Should().BeEquivalentTo("*.cs");
	}

	[Test]
	public void IfAConnectedStringIsSurroundedWithDoubleQuotes_ThenTheQuotesAreRemoved() {
		var args = new[] { "--include=\"*.cs\"" };
		var chain = new ArgumentChain(args);

		var result = chain.SliceString("--include");

		result.Should().BeEquivalentTo("*.cs");
	}

	[Test]
	public void IfAConnectedStringIsSurroundedWithSingleQuotes_ThenTheQuotesAreRemoved() {
		var args = new[] { "--include='*.cs'" };
		var chain = new ArgumentChain(args);

		var result = chain.SliceString("--include");

		result.Should().BeEquivalentTo("*.cs");
	}
}