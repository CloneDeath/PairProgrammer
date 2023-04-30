using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep.ResultSets;

namespace PairProgrammer.Tests.Commands.Grep.ResultSets; 

[TestFixture]
public class FixedSizeQueue_Tests_PushBack : FixedSizeQueue_Tests {
	[Test]
	public void ItemsPushedToTheBackOfTheQueueGetPutInTheEndOfTheResults() {
		var queue = new FixedSizeQueue<int>(5);
		queue.PushBack(1);
		queue.PushBack(2);

		var contents = queue.Contents;

		contents.Should().ContainInConsecutiveOrder(1, 2);
	}

	[Test]
	public void IfAQueueHitsItsSizeLimit_ThenItemsInTheFrontAreRemoved() {
		var queue = new FixedSizeQueue<int>(3);
		queue.PushBack(1);
		queue.PushBack(2);
		queue.PushBack(3);
		queue.PushBack(4);
		
		var contents = queue.Contents.ToList();

		contents.Should().NotContain(1);
		contents.Should().ContainInConsecutiveOrder(2, 3, 4);
	}
}