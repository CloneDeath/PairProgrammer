using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PairProgrammer.GptApi;

public class ChatGptApi
{
	private readonly HttpClient _httpClient;
	private const string _apiUrl = "https://api.openai.com/v1/chat/completions";

	public ChatGptApi(string apiKey)
	{
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
	}

	public async Task<ChatGptResponse> GetChatGptResponseAsync(Message[] messages, GptFunction[]? functions = null)
	{
		var requestBody = new ChatGptRequest
		{
			Model = "gpt-4-0613", 
			Messages = messages,
			Functions = functions,
			MaxTokens = 1000
		};

		var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions());
		var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync(_apiUrl, content);
		if (response.StatusCode == HttpStatusCode.BadRequest) {
			var badResponse = await response.Content.ReadAsStringAsync();
			throw new Exception("BadRequest: " + badResponse + ", Request: " + jsonContent);
		}
		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<ChatGptResponse>(responseBody) 
							 ?? throw new NullReferenceException();
	}
}