using Microsoft.JSInterop;

namespace Pod2Blog.Web.Services.Audio;

public class AudioService : IAudioService
{
    private readonly IJSRuntime _jsRuntime;
    private object? _dotNetHelper;

    public AudioService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void SetDotNetReference(object dotNetHelper)
    {
        _dotNetHelper = dotNetHelper;
    }

    public async Task<bool> InitializeSpeechRecognitionAsync()
    {
        if (_dotNetHelper == null)
            throw new InvalidOperationException("DotNet helper reference not set. Call SetDotNetReference first.");

        try
        {
            return await _jsRuntime.InvokeAsync<bool>("Pod2Blog.initializeSpeechRecognition", _dotNetHelper);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize speech recognition: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StartRecordingAsync(bool enablePauseDetection = false, int pauseSeconds = 2)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("Pod2Blog.startRecording", enablePauseDetection, pauseSeconds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start recording: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StopRecordingAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("Pod2Blog.stopRecording");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to stop recording: {ex.Message}");
            return false;
        }
    }

    public async Task SpeakTextAsync(string text, string voiceName = "", float rate = 1.0f, float pitch = 1.0f)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("speakText", text, voiceName, rate, pitch);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to speak text: {ex.Message}");
        }
    }

    public async Task StopSpeakingAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("stopSpeaking");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to stop speaking: {ex.Message}");
        }
    }

    public async Task<bool> PlayAudioDataAsync(byte[] audioData)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("playAudioData", audioData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to play audio data: {ex.Message}");
            return false;
        }
    }

    public async Task StopAllAudioAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("stopAllAudio");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to stop all audio: {ex.Message}");
        }
    }
}
