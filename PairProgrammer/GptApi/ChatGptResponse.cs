using System;
using System.Text.Json.Serialization;

namespace PairProgrammer.GptApi;

public class ChatGptResponse {
	[JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
	[JsonPropertyName("object")] public string Object { get; set; } = string.Empty;
	[JsonPropertyName("created")] public long Created { get; set; }
	[JsonPropertyName("choices")] public Choice[] Choices { get; set; } = Array.Empty<Choice>();
	[JsonPropertyName("usage")] public Usage Usage { get; set; } = new();
	[JsonPropertyName("error")] public ErrorResponse? Error { get; set; }
}

public class Choice
{
	[JsonPropertyName("index")] public int Index { get; set; }
	[JsonPropertyName("message")] public Message Message { get; set; } = new();
	[JsonPropertyName("finish_reason")] public string FinishReason { get; set; } = string.Empty;
}

public class Usage {
	[JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }
	[JsonPropertyName("completion_tokens")] public int CompletionTokens { get; set; }
	[JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
}
