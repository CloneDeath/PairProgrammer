using System;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep; 

[TestFixture]
public class GrepCommand_Tests_Execute_IgnoreCase {
	[Test]
	public void ItShouldIgnoreCaseWhenCheckingRegex() {
		var input = "HeLlO wOrLd" + Environment.NewLine + "apple";
		var command = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = command.Execute(new[] { "--ignore-case", "hello world" }, input);

		output.Should().Be("HeLlO wOrLd");
	}
}