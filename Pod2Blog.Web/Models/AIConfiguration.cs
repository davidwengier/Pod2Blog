namespace Pod2Blog.Models;

public class AIConfiguration
{
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gpt-4o-mini";
    public string Endpoint { get; set; } = "https://models.inference.ai.azure.com";
    
    // Voice settings
    public bool EnableVoice { get; set; } = false;
    public string VoiceName { get; set; } = ""; // Browser will pick default if empty
    public float VoiceSpeed { get; set; } = 1.0f; // 0.5 to 2.0
    public float VoicePitch { get; set; } = 1.0f; // 0.0 to 2.0
    
    // Auto-pause detection
    public bool EnableAutoPause { get; set; } = true;
    public int AutoPauseSeconds { get; set; } = 3;
}
