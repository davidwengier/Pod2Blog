using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Pod2Blog.Web.Models;

namespace Pod2Blog.Web.Services;

public class OpenAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public OpenAIService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<string> GenerateInterviewQuestionAsync(string topic, List<string> previousQuestions, string? lastUserResponse = null, string? outline = null)
    {
        var systemPrompt = $@"You are a professional podcast interviewer. You're interviewing someone about '{topic}'.";
        
        if (!string.IsNullOrEmpty(outline))
        {
            systemPrompt += $@"

Use this outline to guide your questions:
{outline}

Cover the key points in the outline during the interview, but keep the conversation natural and flowing.";
        }

        systemPrompt += @"

Ask engaging, thoughtful questions that help draw out interesting insights and stories. 
Keep questions conversational and natural. Don't repeat previous questions.
Ask about 5-7 questions total, then suggest wrapping up.";

        var userPrompt = "Generate the next interview question.";
        if (previousQuestions.Any())
        {
            userPrompt += $"\n\nPrevious questions asked:\n{string.Join("\n", previousQuestions)}";
        }
        if (!string.IsNullOrEmpty(lastUserResponse))
        {
            userPrompt += $"\n\nUser's last response: {lastUserResponse}\n\nBuild on this response with your next question.";
        }

        return await CallOpenAIAsync(systemPrompt, userPrompt);
    }

    public async Task<string> GenerateOutlineAsync(string topic)
    {
        var systemPrompt = @"You are an expert content strategist. Generate a concise outline for a blog post on the given topic.
The outline should have 5-6 bullet points covering the key areas to explore.
Format as a simple bullet list using '-' characters.
Keep each point short (5-10 words max).";

        var userPrompt = $"Topic: {topic}\n\nGenerate an outline with 5-6 key bullet points to cover in an interview about this topic.";

        return await CallOpenAIAsync(systemPrompt, userPrompt);
    }

    public async Task<string> GenerateBlogPostAsync(string topic, string conversationTranscript)
    {
        var systemPrompt = @"You are an expert blog writer. Your task is to write an authentic, engaging blog post based on ideas shared during a conversation.

IMPORTANT INSTRUCTIONS:
- Write as if the author sat down and wrote this post themselves, in first person
- DO NOT mention that this came from a conversation, interview, or podcast
- DO NOT quote the conversation or use quotation marks
- DO NOT structure it as a Q&A or dialogue
- Extract the key ideas, insights, examples, and stories from the conversation
- Write in a natural, authentic voice that sounds like the author's own writing
- Use proper markdown formatting with a compelling title (# ), subheadings (##, ###), paragraphs, and natural flow
- Start with an engaging introduction that hooks the reader
- Organize the content logically with clear sections
- End with a meaningful conclusion or call to action
- The tone should be conversational yet polished, as if writing directly to the reader

Think of the conversation as raw material to extract insights from, then write a cohesive, standalone blog post.";

        var userPrompt = $@"Topic: {topic}

Ideas and insights from conversation:
{conversationTranscript}

Write a compelling blog post that captures these ideas in the author's authentic voice. Write as if they're sharing their thoughts directly with readers.";

        return await CallOpenAIAsync(systemPrompt, userPrompt);
    }

    private async Task<string> CallOpenAIAsync(string systemPrompt, string userPrompt)
    {
        var configJson = await _localStorage.GetItemAsync("aiConfig");
        if (string.IsNullOrEmpty(configJson))
        {
            return "⚠️ Error: AI service not configured. Please go to Settings and add your API key.";
        }

        AIConfiguration? config;
        try
        {
            config = JsonSerializer.Deserialize<AIConfiguration>(configJson);
            if (config == null || string.IsNullOrEmpty(config.ApiKey))
            {
                return "⚠️ Error: API key not configured. Please go to Settings and add your API key.";
            }
        }
        catch
        {
            return "⚠️ Error: Invalid configuration. Please check your Settings.";
        }

        var request = new
        {
            model = config.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = 0.7
        };

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{config.Endpoint}/chat/completions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config.ApiKey);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"⚠️ Error calling AI service ({response.StatusCode}): {error}";
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseBody);
            return jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response generated.";
        }
        catch (Exception ex)
        {
            return $"⚠️ Error: {ex.Message}";
        }
    }
}
