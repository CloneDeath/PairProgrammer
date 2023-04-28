using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands;

namespace PairProgrammer.Tests.Commands; 

[TestFixture]
public class CatCommandTests_GetFiles : CatCommandTests {
	[Test]
	public void ItReturnsAllPyFilesInTheDirectory() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/main.py", new MockFileData(
				@"def main():
	print 'hello'
")},
			{"/src/library.py", new MockFileData("// do nothing")},
		});
		var directoryViewer = new DirectoryViewer("src", fileSystem);
		var catCommand = new CatCommand(directoryViewer);

		var output = catCommand.GetFiles("*.py").ToArray();

		output.Should().HaveCount(2);
		output[0].Should().Be("/src/main.py");
		output[1].Should().Be("/src/library.py");
	}
}