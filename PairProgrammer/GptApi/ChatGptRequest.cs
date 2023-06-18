using System;
using System.Text.Json.Serialization;

namespace PairProgrammer.GptApi; 

public class ChatGptRequest {
	[JsonPropertyName("model")] public string Model { get; set; } = string.Empty;
	[JsonPropertyName("messages")] public Message[] Messages { get; set; } = Array.Empty<Message>();
	[JsonPropertyName("max_tokens")] public int? MaxTokens { get; set; }
	[JsonPropertyName("functions")] public GptFunction[]? Functions { get; set; }
}

public class GptFunction {
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("description")] public string? Description { get; set; }
	[JsonPropertyName("parameters")] public object? Parameters { get; set; }
}