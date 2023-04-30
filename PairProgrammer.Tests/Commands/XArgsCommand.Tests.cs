using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands;

namespace PairProgrammer.Tests.Commands; 

[TestFixture]
public class XArgs_Tests {
	[Test]
	public void ItWillRunCatForEachFilePassedIn() {
		var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{ "/src/File.cs", new MockFileData("1") },
			{ "/src/File2.cs", new MockFileData("2") },
			{ "/src/File3.py", new MockFileData("3") },
		});
		var commandExecutor = new CommandExecutor("src", new ProgrammerInterface(), mockFileSystem);

		var output = commandExecutor.ExecuteBash("find . -name '*.cs' | xargs cat");

		output.Should().Be("12");
	}
}