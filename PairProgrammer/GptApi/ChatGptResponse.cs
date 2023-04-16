using System;
using Newtonsoft.Json;

namespace PairProgrammer.GptApi;

public class ChatGptResponse {
	[JsonProperty("id")] public string Id { get; set; } = string.Empty;
	[JsonProperty("object")] public string Object { get; set; } = string.Empty;
	[JsonProperty("created")] public long Created { get; set; }
	[JsonProperty("choices")] public Choice[] Choices { get; set; } = Array.Empty<Choice>();
	[JsonProperty("usage")] public Usage Usage { get; set; } = new();
}

public class Choice
{
	[JsonProperty("index")] public int Index { get; set; }
	[JsonProperty("message")] public Message Message { get; set; } = new();
	[JsonProperty("finish_reason")] public string FinishReason { get; set; } = string.Empty;
}

public class Usage {
	[JsonProperty("prompt_tokens")] public int PromptTokens { get; set; }
	[JsonProperty("completion_tokens")] public int CompletionTokens { get; set; }
	[JsonProperty("total_tokens")] public int TotalTokens { get; set; }
}
