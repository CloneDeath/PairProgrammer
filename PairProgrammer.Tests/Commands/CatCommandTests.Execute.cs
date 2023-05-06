using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands;

namespace PairProgrammer.Tests.Commands; 

[TestFixture]
public class CatCommandTests_Execute : CatCommandTests {
	[Test]
	public void ItCanCatAllPyFiles() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/main.py", new MockFileData(
				@"def main():
	print 'hello'
")},
			{"/src/library.py", new MockFileData("// do nothing")},
		});
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("cat *.py");

		var outputLines = output.Split(Environment.NewLine);
		outputLines.Should().HaveCount(4);
		outputLines[0].Should().Be("def main():");
		outputLines[1].Should().Be("	print 'hello'");
		outputLines[2].Should().Be("");
		outputLines[3].Should().Be("// do nothing");
	}

	[Test]
	public void AbleToCatAFileInTheRootOfTheFileSystem() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/Program.cs", new MockFileData("hello world")},
			{"/src/library.py", new MockFileData("// do nothing")}
		});
		var command = new CatCommand(new DirectoryViewer("src/", fileSystem));

		var output = command.Execute(new[] { "Program.cs" }, string.Empty);

		output.Should().Be("hello world");
	}

	[Test]
	public void IfUsingAFolderGlobPattern_ThenOnlyFilesInTheSubdirectoryAreReturned() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/Program.cs", new MockFileData("a")},
			{"/src/library.cs", new MockFileData("b")},
			{"/src/temp/x.cs", new MockFileData("c")}
		});
		var command = new CatCommand(new DirectoryViewer("src/", fileSystem));

		var output = command.Execute(new[] { "**/*.cs" }, string.Empty);

		output.Should().Be("c");
	}
}