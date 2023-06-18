using System;
using Newtonsoft.Json;

namespace PairProgrammer.GptApi; 

public class ChatGptRequest {
	[JsonProperty("model")] public string Model { get; set; } = string.Empty;
	[JsonProperty("messages")] public Message[] Messages { get; set; } = Array.Empty<Message>();
	[JsonProperty("max_tokens")] public int? MaxTokens { get; set; }
	[JsonProperty("functions")] public GptFunction[]? Functions { get; set; }
}

public class GptFunction {
	[JsonProperty("name")] public string Name { get; set; } = string.Empty;
	[JsonProperty("description")] public string? Description { get; set; }
	[JsonProperty("parameters")] public object? Parameters { get; set; }
}