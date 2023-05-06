using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class ArgumentChain_Tests_SliceString : ArgumentChain_Tests {
	[Test]
	public void ReturnsTheConnectedString() {
		var args = new[] { "-name", "*.cs" };
		var chain = new ArgumentChain(args);

		var result = chain.SliceString("-name");
	}
}