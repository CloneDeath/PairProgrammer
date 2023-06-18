using System;
using System.Text.Json;

namespace PairProgrammer.GptApi.Exceptions; 

public class BadRequestException : Exception {
	public ErrorResponse Response { get; }
	public ChatGptRequest Request { get; }

	public BadRequestException(ErrorResponse response, ChatGptRequest request)
		: base("BadRequest: " + JsonSerializer.Serialize(response) + ", Request: " +
			   JsonSerializer.Serialize(request)) {
		Response = response;
		Request = request;
	}
}