using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

public class CommandExecutorTests_ExecuteBash_grep : CommandExecutorTests_ExecuteBash {
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
}