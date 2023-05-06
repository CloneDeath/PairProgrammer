using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands;

namespace PairProgrammer.Tests.Commands; 

[TestFixture]
public class FindCommand_Tests {
	[Test]
	public void ItIsAbleToFindFiles() {
		var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{ "/src/File.cs", new MockFileData("txt") },
			{ "/src/File2.cs", new MockFileData("txt") },
			{ "/src/File3.py", new MockFileData("txt") },
		});
		var findCommand = new FindCommand(new DirectoryViewer("src", mockFileSystem));

		var output = findCommand.Execute(new[] { ".", "-name", "*.cs" }, string.Empty);

		var expected = string.Join(Environment.NewLine, "./File.cs", "./File2.cs");
		output.Should().Be(expected);
	}
}