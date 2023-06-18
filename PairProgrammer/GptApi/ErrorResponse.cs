using System.Text.Json.Serialization;

namespace PairProgrammer.GptApi; 

public class ErrorResponse {
	[JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
	[JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
	[JsonPropertyName("param")] public string Parameter { get; set; } = string.Empty;
	[JsonPropertyName("code")] public string Code { get; set; } = string.Empty;
}