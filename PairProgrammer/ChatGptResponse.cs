using System;
using Newtonsoft.Json;

namespace PairProgrammer;

public class ChatGptResponse
{
	[JsonProperty("choices")]
	public Choice[] Choices { get; set; } = Array.Empty<Choice>();
}

public class Choice
{
	[JsonProperty("text")]
	public string Text { get; set; } = string.Empty;
}
