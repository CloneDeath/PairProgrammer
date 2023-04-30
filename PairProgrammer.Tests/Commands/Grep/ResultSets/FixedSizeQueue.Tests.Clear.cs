using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep.ResultSets;

namespace PairProgrammer.Tests.Commands.Grep.ResultSets; 

[TestFixture]
public class FixedSizeQueue_Tests_Clear : FixedSizeQueue_Tests {
	[Test]
	public void ItShouldEmptyOutTheQueue() {
		var queue = new FixedSizeQueue<int>(3);
		queue.PushBack(3);

		queue.Clear();

		var contents = queue.Contents;
		contents.Should().BeEmpty();
	}
}