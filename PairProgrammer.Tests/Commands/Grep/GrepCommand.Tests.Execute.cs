using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using NUnit.Framework;
using PairProgrammer.Commands.Grep;

namespace PairProgrammer.Tests.Commands.Grep; 

[TestFixture]
public class GrepCommand_Tests_Execute : GrepCommand_Tests {
	[Test]
	public void ItScansRecursivelyForComments() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/PairProgrammer/Command.cs", new MockFileData("// Summary: it does a thing")},
			{"/src/PairProgrammer.sln", new MockFileData("text")},
			{"/src/PairProgrammer.Tests/DirectoryViewerTests.cs", new MockFileData("text")},
		});
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("grep -r \"//.*Summary:\" .");

		output.Should().Be("./PairProgrammer/Command.cs:// Summary: it does a thing");
	}

	[Test]
	public void IfNoPatternIsProvided_ThenTheUsageIsOutput() {
		IFileSystem fileSystem = new MockFileSystem();
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("grep -r");

		output.Should().Be("Usage: grep [OPTION]... PATTERNS [FILE]...");
	}

	[Test]
	public void ItShouldBeAbleToHandleTheMaxCountArgument() {
		var input = "hello" + Environment.NewLine + "hello";
		var grep = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = grep.Execute(new[] { "-m", "1", "hello" }, input);

		output.Should().Be("hello");
	}

	[Test]
	public void ItShouldBeAbleToGrepTheOutputOfACatCommand() {
		IFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"/src/main.py", new MockFileData(@"def main():
	print 'hello'
")},
			{"/src/library.py", new MockFileData("// do nothing")},
		});
		var commandExecutor = new CommandExecutor("src", new MockProgrammerInterface(), fileSystem);

		var output = commandExecutor.ExecuteBash("cat *.py | grep -m 1 -A 2 'def main():'");

		output.Should().Be("def main():" + Environment.NewLine + "	print 'hello'" + Environment.NewLine);
	}

	[Test]
	public void ItShouldBeAbleToHandleAfterContextCorrectly() {
		var input = string.Join(Environment.NewLine, "apple", "1", 
			"apple", "1", "2", 
			"apple", "1", "2", "3", 
			"apple", "1", "2", "3", "4", "5", "6");
		var command = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = command.Execute(new[] { "-A", "5", "apple" }, input);
		
		var expected = string.Join(Environment.NewLine, "apple", "1", 
			"apple", "1", "2", 
			"apple", "1", "2", "3", 
			"apple", "1", "2", "3", "4", "5");
		output.Should().Be(expected);
	}
	
	[Test]
	public void ItShouldNotPlaceDashesBetweenGapsOfExactlyEqualSize() {
		var input = string.Join(Environment.NewLine, "apple", "1", "2", 
			"apple", "1", "2", "3",
			"apple", "1", "2", "3");
		var command = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = command.Execute(new[] { "-A", "2", "apple" }, input);
		
		var expected = string.Join(Environment.NewLine, "apple", "1", "2",
			"apple", "1", "2",
			"--",
			"apple", "1", "2");
		output.Should().Be(expected);
	}
	
	[Test]
	public void AfterContextShouldNotBringInAnyItemsBeforeTheMatch() {
		var input = string.Join(Environment.NewLine, "1", "apple", "2");
		var command = new GrepCommand(new DirectoryViewer("src", new MockFileSystem()));

		var output = command.Execute(new[] { "-A", "1", "apple" }, input);
		
		var expected = string.Join(Environment.NewLine, "apple", "2");
		output.Should().Be(expected);
	}

	[Test]
	public void ItIsAbleToGetMain() {
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"MySite.sln", new MockFileData("dummy text")},
			{"MyProgram/Program.cs", new MockFileData("    public static async Task Main(string[] args)")}
		});
		var command = new GrepCommand(new DirectoryViewer(".", fileSystem));
		var args = ArgumentSplitter.Split("-r \"static .* Main\" */*.cs");

		var output = command.Execute(args, string.Empty);

		output.Should().Be("./MyProgram/Program.cs:    public static async Task Main(string[] args)");
	}

	[Test]
	public void ItIsAbleToFindADeeplyEmbeddedClassByName() {
		var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
			{"MySite.sln", new MockFileData("dummy text")},
			{"MyProgram/Commands/GrepCommand.cs", new MockFileData("public class GrepCommand : ICommand {")}
		});
		var command = new GrepCommand(new DirectoryViewer(".", fileSystem));
		var args = ArgumentSplitter.Split(@"-r -A 5 -E '\bclass\b.*GrepCommand' --include='*.cs'");

		var output = command.Execute(args, string.Empty);

		output.Should().Be("./MyProgram/Commands/GrepCommand.cs:public class GrepCommand : ICommand {");
	}
}