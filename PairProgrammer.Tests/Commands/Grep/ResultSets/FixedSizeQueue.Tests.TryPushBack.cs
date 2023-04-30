using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep.ResultSets;

namespace PairProgrammer.Tests.Commands.Grep.ResultSets; 

[TestFixture]
public class FixedSizeQueue_Tests_TryPushBack : FixedSizeQueue_Tests {
	[Test]
	public void AddsItemsToTheEndOfTheQueue() {
		var queue = new FixedSizeQueue<int>(3);
		queue.TryPushBack(1);
		queue.TryPushBack(2);
		queue.TryPushBack(3);

		var contents = queue.Contents;

		contents.Should().ContainInOrder(1, 2, 3);
	}

	[Test]
	public void IfTheQueueIsFull_ThenNoItemsAreAdded() {
		var queue = new FixedSizeQueue<int>(2);
		queue.TryPushBack(1);
		queue.TryPushBack(2);
		queue.TryPushBack(3);

		var contents = queue.Contents.ToList();

		contents.Should().ContainInOrder(1, 2);
		contents.Should().NotContain(3);
	}

	[Test]
	public void IfYouTryToPushBack_ButItIsFull_FalseIsReturned() {
		var queue = new FixedSizeQueue<int>(2);
		queue.TryPushBack(1);
		queue.TryPushBack(1);

		var result = queue.TryPushBack(2);

		result.Should().BeFalse();
	}

	[Test]
	public void IfYouTryToPushBackWhenThereIsRoom_ThenTrueIsReturned() {
		var queue = new FixedSizeQueue<int>(20);
		queue.TryPushBack(1);
		queue.TryPushBack(1);

		var result = queue.TryPushBack(2);

		result.Should().BeTrue();
	}
}