using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands;

namespace PairProgrammer.Tests.Commands; 

[TestFixture]
public class GrepCommandTests {
	[Test]
	public void ItScansRecursivelyForComments() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/PairProgrammer/Command.cs", new MockFileData("// Summary: it does a thing")},
			{"/src/PairProgrammer.sln", new MockFileData("text")},
			{"/src/PairProgrammer.Tests/DirectoryViewerTests.cs", new MockFileData("text")},
		});
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("grep -r \"//.*Summary:\" .");

		output.Should().Be("./PairProgrammer/Command.cs: // Summary: it does a thing");
	}

	[Test]
	public void IfNoPatternIsProvided_ThenTheUsageIsOutput() {
		IFileSystem fileSystem = new MockFileSystem();
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("grep -r");

		output.Should().Be("Usage: grep [OPTION]... PATTERNS [FILE]...");
	}

	[Test]
	public void ItShouldBeAbleToHandleTheMaxCountArgument() {
		var input = "hello" + Environment.NewLine + "hello";
		var grep = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = grep.Execute(new[] { "-m", "1" }, input);

		output.Should().Be("hello");
	}

	[Test]
	public void ItShouldBeAbleToGrepTheOutputOfACatCommand() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/main.py", new MockFileData(@"def main():
	print 'hello'
")},
			{"/src/library.py", new MockFileData("// do nothing")},
		});
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("cat *.py | grep -m 1 -A 5 'def main():'");

		output.Should().Be("def main():" + Environment.NewLine + "	print 'hello'" + Environment.NewLine);
	}
}