namespace Pod2Blog.Web.Services.Audio;

public interface IAudioService
{
    Task<bool> InitializeSpeechRecognitionAsync();
    Task<bool> StartRecordingAsync(bool enablePauseDetection = false, int pauseSeconds = 2);
    Task<bool> StopRecordingAsync();
    Task SpeakTextAsync(string text, string voiceName = "", float rate = 1.0f, float pitch = 1.0f);
    Task StopSpeakingAsync();
    Task<bool> PlayAudioDataAsync(byte[] audioData);
    Task StopAllAudioAsync();
    void SetDotNetReference(object dotNetHelper);
}
