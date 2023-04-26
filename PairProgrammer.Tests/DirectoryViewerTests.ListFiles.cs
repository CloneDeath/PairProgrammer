using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

public class DirectoryViewerTests_ListFiles : DirectoryViewerTests {
	[Test]
	public void ItShouldOnlyReturnFilesInTheSpecifiedDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/tmp/b.txt", new MockFileData("text")},
			{"/src/a.txt", new MockFileData("text")}
		});
		var directoryViewer = new DirectoryViewer("src", fileSystem);

		var files = directoryViewer.ListFiles(".").ToArray();

		files.Should().HaveCount(1);
		files.Should().NotContain("/src/b.txt");
	}
}