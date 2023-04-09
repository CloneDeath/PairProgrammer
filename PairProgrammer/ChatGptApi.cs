using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PairProgrammer;

public class ChatGptApi
{
	private readonly HttpClient _httpClient;
	private const string ApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

	public ChatGptApi(string apiKey)
	{
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
	}

	public async Task<string> GetChatGptResponseAsync(string prompt)
	{
		var requestBody = new
		{
			prompt,
			max_tokens = 100,
			n = 1,
			stop = Array.Empty<string>(),
			temperature = 0.7
		};

		var jsonContent = JsonConvert.SerializeObject(requestBody);
		var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync(ApiUrl, content);
		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		var responseObject = JsonConvert.DeserializeObject<ChatGptResponse>(responseBody) ?? throw new NullReferenceException();
		var chatGptResponse = responseObject.Choices[0].Text;

		return chatGptResponse.Trim();
	}
}