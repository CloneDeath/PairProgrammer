using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

		var jsonContent = JsonConvert.SerializeObject(requestBody);
		var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync(_apiUrl, content);
		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<ChatGptResponse>(responseBody) 
							 ?? throw new NullReferenceException();
	}
}