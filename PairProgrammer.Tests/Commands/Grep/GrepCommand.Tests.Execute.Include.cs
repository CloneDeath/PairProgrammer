using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep; 

[TestFixture]
public class GrepCommand_Tests_Execute_Include : GrepCommand_Tests {
	[Test]
    public void ItIncludesFilesBasedOnPattern()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {"MySite.sln", new MockFileData("dummy text")},
            {"MyProgram/Program.cs", new MockFileData("    public static async Task Main(string[] args)")},
            {"MyProgram/OtherClass.cs", new MockFileData("public class OtherClass { }")}
        });
        var command = new GrepCommand(new DirectoryViewer(".", fileSystem));
        var args = ArgumentSplitter.Split("-r --include \"*.cs\" \"public class\"");

        var output = command.Execute(args, string.Empty);

        output.Should().Be("./MyProgram/OtherClass.cs:public class OtherClass { }");
    }

    [Test]
    public void ItExcludesFilesNotMatchingIncludePattern()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {"MySite.sln", new MockFileData("dummy text")},
            {"MyProgram/Program.cs", new MockFileData("    public static async Task Main(string[] args)")},
            {"MyProgram/OtherClass.txt", new MockFileData("public class OtherClass { }")}
        });
        var command = new GrepCommand(new DirectoryViewer(".", fileSystem));
        var args = ArgumentSplitter.Split("-r --include \"*.cs\" \"public class\"");

        var output = command.Execute(args, string.Empty);

        output.Should().BeEmpty();
    }

    [Test]
    public void ItCombinesIncludePatternWithOtherFlags()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {"MySite.sln", new MockFileData("dummy text")},
            {"MyProgram/Program.cs", new MockFileData("    public static async Task Main(string[] args)")},
            {"MyProgram/OtherClass.cs", new MockFileData("public class OtherClass { }")},
            {"MyProgram/AnotherClass.cs", new MockFileData("public class AnotherClass { }")}
        });
        var command = new GrepCommand(new DirectoryViewer(".", fileSystem));
        var args = ArgumentSplitter.Split("-r --include \"*.cs\" -i \"PUBLIC CLASS\"");

        var output = command.Execute(args, string.Empty);

        output.Should().Be("./MyProgram/OtherClass.cs:public class OtherClass { }\n./MyProgram/AnotherClass.cs:public class AnotherClass { }");
    }
}