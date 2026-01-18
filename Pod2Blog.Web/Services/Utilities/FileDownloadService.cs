using Microsoft.JSInterop;

namespace Pod2Blog.Web.Services.Utilities;

public interface IFileDownloadService
{
    Task DownloadFileAsync(string filename, string content);
}

public class FileDownloadService : IFileDownloadService
{
    private readonly IJSRuntime _jsRuntime;

    public FileDownloadService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadFileAsync(string filename, string content)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("Pod2Blog.downloadFile", filename, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download file: {ex.Message}");
        }
    }
}
