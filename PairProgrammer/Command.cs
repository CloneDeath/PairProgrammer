using Newtonsoft.Json;

namespace PairProgrammer;

public class Command
{
	[JsonProperty("list")]
	public string? List { get; set; }

	[JsonProperty("access")]
	public string? Access { get; set; }

	[JsonProperty("message")]
	public string? Message { get; set; }
	
	
	// TODO:
	// run package add, `dotnet add package Microsoft.Extensions.Http`
	// list installed packages, to see if something is installed
	// Update a file
	// Create a file
}
