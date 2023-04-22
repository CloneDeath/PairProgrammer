using System;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests;

public class CommandExecutorTests_ExecuteBash : CommandExecutorTests {
	[Test]
	public void AbleToExecute_date() {
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), new MockFileSystem());

		var output = commandExecutor.ExecuteBash("date");

		var now = DateTime.Now;
		var dayOfWeek = now.DayOfWeek switch {
			DayOfWeek.Saturday => "Sat",
			DayOfWeek.Sunday => "Sun",
			DayOfWeek.Monday => "Mon",
			DayOfWeek.Tuesday => "Tue",
			DayOfWeek.Wednesday => "Wed",
			DayOfWeek.Thursday => "Thu",
			DayOfWeek.Friday => "Fri",
			_ => throw new ArgumentOutOfRangeException()
		};
		var monthName = now.Month switch {
			1 => "Jan",
			2 => "Feb",
			3 => "Mar",
			4 => "Apr",
			5 => "May",
			6 => "Jun",
			7 => "Jul",
			8 => "Aug",
			9 => "Sep",
			10 => "Oct",
			11 => "Nov",
			12 => "Dec",
			_ => throw new ArgumentOutOfRangeException()
		};
		var amPm = now.Hour < 12 ? "AM" : "PM";
		var offset = (now - DateTime.UtcNow).TotalHours;
		output.Should().Be($"{dayOfWeek} {monthName} {now.Day} {now.Hour%12:00}:{now.Minute:00}:{now.Second:00} {amPm} {offset:00} {now.Year}");
	}
}