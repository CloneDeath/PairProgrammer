using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep.ResultSets;

namespace PairProgrammer.Tests.Commands.Grep.ResultSets; 

[TestFixture]
public class FixedSizeQueue_Tests_Any : FixedSizeQueue_Tests {
	[Test]
	public void ItReturnsFalseWhenEmpty() {
		var queue = new FixedSizeQueue<int>(3);

		var any = queue.Any();

		any.Should().BeFalse();
	}

	[Test]
	public void ItReturnsTrueIfItHasAnyItems() {
		var queue = new FixedSizeQueue<int>(3);
		queue.PushBack(1);

		var any = queue.Any();

		any.Should().BeTrue();
	}
}