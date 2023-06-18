using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class FileSystemAccessTests_ReadFile : FileSystemAccessTests {
	[Test]
	public void CanAccessAFile() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/main.py", new MockFileData(
				@"def main():
	print 'hello'
")},
			{"/src/library.py", new MockFileData("// do nothing")},
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var output = directoryViewer.ReadFile("/src/library.py");

		output.Should().Be("// do nothing");
	}

	[Test]
	public void IfScopeEndsWithASlash_ThenItShouldStillBeAbleToAccessFilesInTheRootOfTheDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"src/main.py", new MockFileData("hello world")},
			{"src/library.py", new MockFileData("// do nothing")},
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var output = directoryViewer.ReadFile("library.py");

		output.Should().Be("// do nothing");
	}
}