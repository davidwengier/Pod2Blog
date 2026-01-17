namespace Pod2Blog.Services;

public interface IAIService
{
    Task<string> GenerateInterviewQuestionAsync(string topic, List<string> previousQuestions, string? lastUserResponse = null, string? outline = null);
    Task<string> GenerateBlogPostAsync(string topic, string conversationTranscript);
    Task<string> GenerateOutlineAsync(string topic);
}
