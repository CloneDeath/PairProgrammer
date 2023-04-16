using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace PairProgrammer.GptApi;

public class Message {
	[JsonProperty("role")] public Role Role { get; set; } = Role.User;
	[JsonProperty("content")] public string Content { get; set; } = string.Empty;
}

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum Role
{
	User,
	System,
	Assistant
}