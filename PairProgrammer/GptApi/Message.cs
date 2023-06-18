using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace PairProgrammer.GptApi;

public class Message {
	[JsonProperty("role")] public Role Role { get; set; } = Role.User;
	[JsonProperty("content")] public string? Content { get; set; } = string.Empty;
	[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)] public string? Name { get; set; }
	[JsonProperty("function_call", NullValueHandling = NullValueHandling.Ignore)] public FunctionCall? FunctionCall { get; set; }
}

public class FunctionCall {
	[JsonProperty("name")] public string Name { get; set; } = string.Empty;
	[JsonProperty("arguments")] public string Arguments { get; set; } = string.Empty;
}

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum Role
{
	User,
	System,
	Assistant,
	Function
}