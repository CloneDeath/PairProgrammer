using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

public class DirectoryViewerTests_List : DirectoryViewerTests {
	[Test]
	public void ItIsAbleToListFilesInAFolder() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/PairProgrammer/Command.cs", new MockFileData("text")},
			{"/src/PairProgrammer.sln", new MockFileData("text")},
			{"/src/PairProgrammer.Tests/DirectoryViewerTests.cs", new MockFileData("text")},
		});
		var directoryViewer = new DirectoryViewer("src", fileSystem);

		var files = directoryViewer.List("").ToArray();

		files.Should().Contain("/src/PairProgrammer");
		files.Should().Contain("/src/PairProgrammer.sln");
		files.Should().Contain("/src/PairProgrammer.Tests");
	}

	[Test]
	public void ItShouldNotContainFilesOutsideOfTheSpecifiedDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/dst/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new DirectoryViewer("src", fileSystem);

		var files = directoryViewer.List("").ToArray();

		files.Should().NotContain("/src/b.txt");
	}
}