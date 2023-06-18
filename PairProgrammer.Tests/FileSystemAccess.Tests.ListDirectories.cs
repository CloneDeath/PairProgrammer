using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

public class FileSystemAccessTests_ListFolders : FileSystemAccessTests {
	[Test]
	public void ItIsAbleToListDirectoriesInADirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/PairProgrammer/Command.cs", new MockFileData("text")},
			{"/src/PairProgrammer.sln", new MockFileData("text")},
			{"/src/PairProgrammer.Tests/DirectoryViewerTests.cs", new MockFileData("text")},
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var files = directoryViewer.ListDirectories("", false).ToArray();

		files.Should().HaveCount(2);
		files.Should().Contain("/PairProgrammer");
		files.Should().Contain("/PairProgrammer.Tests");
	}

	[Test]
	public void ItShouldNotContainDirectoriesOutsideOfTheSpecifiedDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/dst/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var files = directoryViewer.ListDirectories("", false).ToArray();

		files.Should().NotContainMatch("*dst*");
	}
	
	
	[Test]
	public void ItShouldOnlyReturnDirectoriesInTheSpecifiedDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/a.txt", new MockFileData("text")},
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/tmp/ext/c.txt", new MockFileData("text")},
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var files = directoryViewer.ListDirectories(".", false).ToArray();

		files.Should().HaveCount(1);
		files.Should().NotContain("*ext*");
	}

	[Test]
	public void IfTheRootEndsInASlash_ItCanListDirectoriesInTheRootOfTheRepo() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListDirectories(".", false).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/tmp");
	}

	[Test]
	public void ItCanReturnDirectoriesRecursively() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/tmp/ext/c.txt", new MockFileData("text")},
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListDirectories(".", true).ToArray();

		files.Should().HaveCount(2);
		files.Should().Contain("/tmp");
		files.Should().Contain("/tmp/ext");
	}

	[Test]
	public void ItCanRetrieveDirectoriesInASubDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/a/correct/1.txt", new MockFileData("text")},
			{"/src/a/correct/2.txt", new MockFileData("text")},
			{"/src/b/wrong/3.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListDirectories("./a", false).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/a/correct");
	}

	[Test]
	public void ItShouldAssumeSlashToBeLocalToTheRootOfTheRepo() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/a/left/1.txt", new MockFileData("text")},
			{"/src/a/left/2.txt", new MockFileData("text")},
			{"/src/b/right/3.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListDirectories("/", true).ToArray();

		files.Should().HaveCount(4);
		files.Should().Contain("/a");
		files.Should().Contain("/a/left");
		files.Should().Contain("/b");
		files.Should().Contain("/b/right");
	}
}