using Newtonsoft.Json;

namespace PairProgrammer;

public class Command
{
	[JsonProperty("chat")]
	public string? Chat { get; set; }

	[JsonProperty("bash")]
	public string? Bash { get; set; }
	
	[JsonProperty("comment")]
	public string? Comment { get; set; }
}
