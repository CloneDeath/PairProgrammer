using System.Text.Json;
using System.Text.Json.Serialization;

namespace PairProgrammer.GptApi;

public class Message {
	[JsonPropertyName("role")] public Role Role { get; set; } = Role.User;
	[JsonPropertyName("content")] public string? Content { get; set; } = string.Empty;
	
	[JsonPropertyName("name")] 
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
	public string? Name { get; set; }
	
	[JsonPropertyName("function_call")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
	public FunctionCall? FunctionCall { get; set; }
}

public class FunctionCall {
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("arguments")] public string Arguments { get; set; } = string.Empty;
}

[JsonConverter(typeof(LowerCaseEnumConverter))]
public enum Role
{
	[JsonPropertyName("user")] User,
	[JsonPropertyName("system")] System,
	[JsonPropertyName("assistant")] Assistant,
	[JsonPropertyName("function")] Function
}

public class LowerCaseEnumConverter : JsonStringEnumConverter {
	public LowerCaseEnumConverter() : base(new LowerCaseNamingPolicy()) { }
}

public class LowerCaseNamingPolicy : JsonNamingPolicy {
	public override string ConvertName(string name) => name.ToLower();
}