using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

public class FileSystemAccessTests_ListFiles : FileSystemAccessTests {
	[Test]
	public void ItIsAbleToListFilesInAFolder() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/PairProgrammer/Command.cs", new MockFileData("text")},
			{"/src/PairProgrammer.sln", new MockFileData("text")},
			{"/src/PairProgrammer.Tests/DirectoryViewerTests.cs", new MockFileData("text")},
			{"/src/readme.md", new MockFileData("hello world")},
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var files = directoryViewer.ListFiles("", false).ToArray();

		files.Should().HaveCount(2);
		files.Should().Contain("/PairProgrammer.sln");
		files.Should().Contain("/readme.md");
	}

	[Test]
	public void ItShouldNotContainFilesOutsideOfTheSpecifiedDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/dst/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var files = directoryViewer.ListFiles("", false).ToArray();

		files.Should().NotContainMatch("*b.txt");
	}
	
	
	[Test]
	public void ItShouldOnlyReturnFilesInTheSpecifiedDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src", fileSystem);

		var files = directoryViewer.ListFiles(".", false).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/a.txt");
	}

	[Test]
	public void IfTheRootEndsInASlash_ItCanListFilesInTheRootOfTheRepo() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListFiles(".", false).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/a.txt");
	}

	[Test]
	public void IfRecursiveIsTrue_ThenItShouldReturnChildFiles() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListFiles(".", true).ToArray();

		files.Should().HaveCount(2);
		files.Should().Contain("/a.txt");
		files.Should().Contain("/tmp/b.txt");
	}

	[Test]
	public void ItShouldReturnFilesInTheSpecifiedSubdirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/a/correct.txt", new MockFileData("text")},
			{"/src/b/wrong.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListFiles("./a/", true).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/a/correct.txt");
	}
	
	[Test]
	public void ItShouldAssumeSlashToBeLocalToTheRootOfTheRepo() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/a/left/1.txt", new MockFileData("text")},
			{"/src/a/left/2.txt", new MockFileData("text")},
			{"/src/b/right/3.txt", new MockFileData("text")}
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListFiles("/", true).ToArray();

		files.Should().HaveCount(3);
		files.Should().Contain("/a/left/1.txt");
		files.Should().Contain("/a/left/2.txt");
		files.Should().Contain("/b/right/3.txt");
	}

	[Test]
	public void ItShouldNotReturnHiddenFiles() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/.gitignore", new MockFileData("text")},
			{"/src/code.js", new MockFileData("text")},
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListFiles("/", false).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/code.js");
	}

	[Test]
	public void ItShouldNotReturnFilesInHiddenFolders() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/a/code.js", new MockFileData("text")},
			{"/src/.git/obj.txt", new MockFileData("text")},
		});
		var directoryViewer = new FileSystemAccess("src/", fileSystem);

		var files = directoryViewer.ListFiles("/", true).ToArray();

		files.Should().HaveCount(1);
		files.Should().Contain("/a/code.js");
	}
}