using System;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests.Commands; 

[TestFixture]
public class DateCommandTests {
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
		var monthName = GetShortMonthName(now);
		var amPm = GetAMorPM(now);
		var offset = (now - DateTime.UtcNow).TotalHours;
		output.Should().Be($"{dayOfWeek} {monthName} {now.Day} {now.Hour%12:00}:{now.Minute:00}:{now.Second:00} {amPm} {offset:00} {now.Year}");
	}

	private static string GetShortMonthName(DateTime now) {
		return GetMonthName(now)[..3];
	}

	private static string GetMonthName(DateTime now) {
		return now.Month switch {
			1 => "January",
			2 => "February",
			3 => "March",
			4 => "April",
			5 => "May",
			6 => "June",
			7 => "July",
			8 => "August",
			9 => "September",
			10 => "October",
			11 => "November",
			12 => "December",
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private string GetAMorPM(DateTime dateTime) {
		return dateTime.Hour < 12 ? "AM" : "PM";
	}

	[Test]
	public void AbleToExecute_date_WithPercentRParam() {
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), new MockFileSystem());

		var output = commandExecutor.ExecuteBash("date +'%r'");

		var now = DateTime.Now;
		var amPm = GetAMorPM(now);
		output.Should().Be($"{now.Hour:00}:{now.Minute:00}:{now.Second:00} {amPm}");
	}

	[Test]
	public void ItShouldStillWorkEvenWithoutTheQuotes() {
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), new MockFileSystem());

		var output = commandExecutor.ExecuteBash("date +%r");

		var now = DateTime.Now;
		var amPm = GetAMorPM(now);
		output.Should().Be($"{now.Hour:00}:{now.Minute:00}:{now.Second:00} {amPm}");
	}

	[Test]
	public void IsShouldBeAbleToGetTheDate() {
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), new MockFileSystem());

		var output = commandExecutor.ExecuteBash("date \"+%A, %B %d, %Y\"");

		var now = DateTime.Now;
		var monthName = GetMonthName(now);
		output.Should().Be($"{now.DayOfWeek}, {monthName} {now.Day:00}, {now.Year}");
	}

	[Test]
	public void WhenAnInvalidOperandIsPassedIn_ItShouldReturnAnError() {
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), new MockFileSystem());

		var output = commandExecutor.ExecuteBash("date +%A, %B %d, %Y");

		output.Should().Be("date: extra operand '%B'");
	}
}