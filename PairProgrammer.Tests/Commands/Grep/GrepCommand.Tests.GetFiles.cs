using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep; 

[TestFixture]
public class GrepCommand_Tests_GetFiles : GrepCommand_Tests {
	[Test]
	public void IfTheScopeIsADirectory_AndRecursiveIsTrue_ThenAllFilesAreReturnedUnderTheScope() {
		var filesystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{ "src/a.txt", new MockFileData("") },
			{ "src/s/b.txt", new MockFileData("") },
			{ "src/s/s/c.txt", new MockFileData("") },
		});
		var command = new GrepCommand(new DirectoryViewer("src", filesystem));

		var result = command.GetFiles(".", true).ToArray();

		result.Should().HaveCount(3);
		result.Should().Contain("/src/a.txt");
		result.Should().Contain("/src/s/b.txt");
		result.Should().Contain("/src/s/s/c.txt");
	}

	[Test]
	public void IfTheScopeIsAFile_AndRecursiveIsFalse_ThenOnlyTheSpecifiedFileIsReturned() {
		var filesystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{ "src/a.txt", new MockFileData("") },
			{ "src/b.txt", new MockFileData("") },
		});
		var command = new GrepCommand(new DirectoryViewer("src", filesystem));

		var result = command.GetFiles("a.txt", false).ToArray();

		result.Should().HaveCount(1);
		result.Should().Contain("a.txt");
	}

	[Test]
	public void IfTheScopeIsWildcard_AndRecursiveIsFalse_ThenAllMatchingFilesInTheCurrentDirectoryAreReturned() {
		var filesystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{ "src/a.txt", new MockFileData("") },
			{ "src/b.txt", new MockFileData("") },
			{ "src/c.md", new MockFileData("") },
		});
		var command = new GrepCommand(new DirectoryViewer("src", filesystem));

		var result = command.GetFiles("*.txt", false).ToArray();

		result.Should().HaveCount(2);
		result.Should().Contain("/src/a.txt");
		result.Should().Contain("/src/b.txt");
	}
	
	[Test]
	public void IfTheScopeIsWildcard_AndRecursiveIsTrue_ThenRecursiveIsIgnored_AndOnlyLocalFilesAreReturned() {
		var filesystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{ "src/a.txt", new MockFileData("") },
			{ "src/b.txt", new MockFileData("") },
			{ "src/s/c.txt", new MockFileData("") },
			{ "src/s/d.md", new MockFileData("") },
		});
		var command = new GrepCommand(new DirectoryViewer("src", filesystem));

		var result = command.GetFiles("*.txt", true).ToArray();

		result.Should().HaveCount(2);
		result.Should().Contain("/src/a.txt");
		result.Should().Contain("/src/b.txt");
	}
}