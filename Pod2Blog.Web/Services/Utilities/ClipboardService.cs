using Microsoft.JSInterop;

namespace Pod2Blog.Web.Services.Utilities;

public interface IClipboardService
{
    Task<bool> CopyToClipboardAsync(string text);
}

public class ClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> CopyToClipboardAsync(string text)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("Pod2Blog.copyToClipboard", text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to copy to clipboard: {ex.Message}");
            return false;
        }
    }
}
