namespace Pod2Blog.Web.Services;

public enum BlogFidelity
{
    StrictlyFaithful,   // Only what was said in the interview
    Balanced,           // Some elaboration and polish
    Enhanced            // Full elaboration with additional context
}

public interface IAIService
{
    Task<string> GenerateInterviewQuestionAsync(string topic, List<string> previousQuestions, string? lastUserResponse = null, string? outline = null);
    Task<string> GenerateBlogPostAsync(string topic, string conversationTranscript, BlogFidelity fidelity = BlogFidelity.Balanced);
    Task<string> GenerateOutlineAsync(string topic);
}
