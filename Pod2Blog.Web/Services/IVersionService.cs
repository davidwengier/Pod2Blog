namespace Pod2Blog.Web.Services;

public interface IVersionService
{
    Task<string?> GetCurrentVersionAsync();
    Task<bool> CheckForUpdateAsync();
    event Action? OnUpdateAvailable;
}
