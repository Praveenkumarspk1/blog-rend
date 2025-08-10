using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace BlogSpace.Client.Services;

public interface IGeminiService
{
    Task<List<string>> GenerateBlogIdeas(string topic);
    Task<string> ImproveBlogContent(string content, string instructions);
    Task<List<string>> GenerateSEOKeywords(string title, string content);
    Task<string> GenerateBlogSummary(string content);
    Task<string> GetChatResponse(string message);
}

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiBaseUrl;

    public GeminiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey");
        _apiBaseUrl = configuration["Gemini:ApiBaseUrl"] ?? throw new ArgumentNullException("Gemini:ApiBaseUrl");
    }

    public async Task<List<string>> GenerateBlogIdeas(string topic)
    {
        var prompt = $"Generate 5 creative and engaging blog post ideas about {topic}. " +
                    "Each idea should be unique and appeal to a broad audience. " +
                    "Format each idea as a complete title that would attract readers.";

        var response = await GetGeminiResponse(prompt);
        return ParseListResponse(response);
    }

    public async Task<string> ImproveBlogContent(string content, string instructions)
    {
        var prompt = $"Improve the following blog content according to these instructions: {instructions}\n\n{content}";
        return await GetGeminiResponse(prompt);
    }

    public async Task<List<string>> GenerateSEOKeywords(string title, string content)
    {
        var prompt = $"Generate 10 relevant SEO keywords for a blog post with the following title and content:\n\n" +
                    $"Title: {title}\n\n{content}\n\n" +
                    "Format each keyword as a single line without numbers or bullet points.";

        var response = await GetGeminiResponse(prompt);
        return ParseListResponse(response);
    }

    public async Task<string> GenerateBlogSummary(string content)
    {
        var prompt = "Generate a concise and engaging summary of the following blog post content " +
                    "that captures the main points and encourages readers to read more:\n\n" + content;

        return await GetGeminiResponse(prompt);
    }

    public async Task<string> GetChatResponse(string message)
    {
        var prompt = $"You are a helpful AI writing assistant. Respond to the following message in a friendly and professional manner:\n\n{message}";
        return await GetGeminiResponse(prompt);
    }

    private async Task<string> GetGeminiResponse(string prompt)
    {
        try
        {
            var requestUrl = $"{_apiBaseUrl}?key={_apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(requestUrl, requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"AI service error: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get AI response: {ex.Message}");
        }
    }

    private List<string> ParseListResponse(string response)
    {
        return response
            .Split('\n')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => Regex.Replace(line, @"^\d+\.\s*", "").Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }
}

public class GeminiResponse
{
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    public GeminiContent? Content { get; set; }
}

public class GeminiContent
{
    public List<GeminiPart>? Parts { get; set; }
}

public class GeminiPart
{
    public string? Text { get; set; }
} 