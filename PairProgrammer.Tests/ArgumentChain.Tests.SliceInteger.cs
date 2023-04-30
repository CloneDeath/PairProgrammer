using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class ArgumentChain_Tests_SliceInteger : ArgumentChain_Tests {
	[Test]
	public void ItIsAbleToTakeAnIntegerArgumentFromAChain() {
		var args = new[] { "-f", "-m", "5" };
		var chain = new ArgumentChain(args);

		var value = chain.SliceInteger("-m");

		value.Should().Be(5);
	}

	[Test]
	public void ItIsAbleToTakeOneOfMultipleFlags() {
		var args = new[] { "-f", "--max-count", "11" };
		var chain = new ArgumentChain(args);

		var value = chain.SliceInteger("-m", "--max-count");

		value.Should().Be(11);
	}

	[Test]
	public void ItCanDetectArgumentsWithEqualAssignments() {
		var args = new[] { "-f", "--max-count=2", "11" };
		var chain = new ArgumentChain(args);

		var value = chain.SliceInteger("-m", "--max-count");

		value.Should().Be(2);
	}

	[Test]
	public void IfAnArgumentExistsOnce_ThenItCanOnlyBeDetectedOnce() {
		var args = new[] { "-f", "--max-count=2", "11" };
		var chain = new ArgumentChain(args);
		chain.SliceInteger("-m", "--max-count");
		
		var value = chain.SliceInteger("-m", "--max-count");

		value.Should().BeNull();
	}

	[Test]
	public void IfAnArgumentExistsTwice_ThenTheSecondCallShouldReturnTheSecondArgument() {
		var args = new[] { "-f", "--max-count=2", "-m", "18" };
		var chain = new ArgumentChain(args);
		chain.SliceInteger("-m", "--max-count");
		
		var value = chain.SliceInteger("-m", "--max-count");

		value.Should().Be(18);
	}
}