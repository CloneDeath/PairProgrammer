using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class ArgumentChain_Tests_SliceFlag : ArgumentChain_Tests {
	[Test]
	public void ItIsAbleToSliceABooleanFlagFromTheArguments() {
		var args = new[] { "-f", "-m", "5" };
		var chain = new ArgumentChain(args);

		var flag = chain.SliceFlag("-f");

		flag.Should().BeTrue();
	}

	[Test]
	public void ItShouldReturnFalseIfNoFlagIsFound() {
		var args = new[] { "-f", "-m", "5" };
		var chain = new ArgumentChain(args);

		var flag = chain.SliceFlag("-q");

		flag.Should().BeFalse();
	}

	[Test]
	public void OnceAFlagIsSliced_ItShouldNotBeDetectedAnymore() {
		var args = new[] { "-f", "-m", "5" };
		var chain = new ArgumentChain(args);
		chain.SliceFlag("-f");
		
		var flag = chain.SliceFlag("-f");

		flag.Should().BeFalse();
	}

	[Test]
	public void WhenSlicingMultipleFlags_IfBothArePresent_ThenItIsTrue() {
		var args = new[] { "-c", "--count", "-f" };
		var chain = new ArgumentChain(args);
		
		var flag = chain.SliceFlag("-c", "--count");

		flag.Should().BeTrue();
	}
}