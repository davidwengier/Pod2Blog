namespace Pod2Blog.Models;

public class PodcastSession
{
    public string Topic { get; set; } = "";
    public string? Outline { get; set; }
    public List<ConversationTurn> Conversation { get; set; } = new();
    public string? GeneratedBlogPost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ConversationTurn
{
    public string Speaker { get; set; } = ""; // "AI" or "User"
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
