using FluentAssertions;
using NUnit.Framework;

namespace PairProgrammer.Tests; 

[TestFixture]
public class GlobToRegex_Tests_Convert : GlobToRegex_Tests {
	[Test]
	public void ItShouldBeAbleToConvertWildcards() {
		const string pattern = "*.cs";

		var regex = GlobToRegex.Convert(pattern);

		var matches = regex.IsMatch("hello.cs");
		matches.Should().BeTrue();
	}

	[Test]
	public void FolderWildcardShouldAlsoWork() {
		const string pattern = "*/*.cs";
		
		var regex = GlobToRegex.Convert(pattern);

		var matches = regex.IsMatch("src/hello.cs");
		matches.Should().BeTrue();
	}

	[Test]
	public void ItShouldNotReturnValuesOutsideOfFoldersIfOneIsSpecified() {
		const string pattern = "*/*.cs";
		
		var regex = GlobToRegex.Convert(pattern);

		var matches = regex.IsMatch("hello.cs");
		matches.Should().BeFalse();
	}

	[Test]
	public void ItShouldNotReturnFoldersThatAreDeeperThanWhatIsSpecified() {
		const string pattern = "*/*.cs";
		
		var regex = GlobToRegex.Convert(pattern);

		var matches = regex.IsMatch("src/a/hello.cs");
		matches.Should().BeFalse();
	}

	[Test]
	public void ItShouldNotReturnChildFolderItemsIfNoneAreSpecified() {
		const string pattern = "*.cs";

		var regex = GlobToRegex.Convert(pattern);

		var matches = regex.IsMatch("src/hello.cs");
		matches.Should().BeFalse(); 
	}
}