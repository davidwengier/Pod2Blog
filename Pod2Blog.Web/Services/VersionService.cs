using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Pod2Blog.Web.Services;

public class VersionService : IVersionService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly Timer? _timer;
    private string? _currentVersion;
    private const string VERSION_STORAGE_KEY = "app_version";
    private const int CHECK_INTERVAL_MINUTES = 5;

    public event Action? OnUpdateAvailable;

    public VersionService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        
        // Check for updates periodically (every 5 minutes)
        _timer = new Timer(async _ => await CheckForUpdateAsync(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(CHECK_INTERVAL_MINUTES));
    }

    public async Task<string?> GetCurrentVersionAsync()
    {
        if (_currentVersion != null)
            return _currentVersion;

        try
        {
            // Add cache-busting query parameter
            var versionInfo = await _httpClient.GetFromJsonAsync<VersionInfo>($"version.json?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
            _currentVersion = versionInfo?.Version;
            
            // Store version on first load
            var storedVersion = await _localStorage.GetItemAsync(VERSION_STORAGE_KEY);
            if (storedVersion == null && _currentVersion != null)
            {
                await _localStorage.SetItemAsync(VERSION_STORAGE_KEY, _currentVersion);
            }
            
            return _currentVersion;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get current version: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CheckForUpdateAsync()
    {
        try
        {
            // Get stored version
            var storedVersion = await _localStorage.GetItemAsync(VERSION_STORAGE_KEY);
            if (storedVersion == null)
            {
                // First run, store current version
                await GetCurrentVersionAsync();
                return false;
            }

            // Fetch latest version with cache-busting
            var versionInfo = await _httpClient.GetFromJsonAsync<VersionInfo>($"version.json?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");
            var latestVersion = versionInfo?.Version;

            if (latestVersion != null && latestVersion != storedVersion)
            {
                Console.WriteLine($"Update available! Current: {storedVersion}, Latest: {latestVersion}");
                OnUpdateAvailable?.Invoke();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to check for updates: {ex.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private class VersionInfo
    {
        [JsonPropertyName("version")]
        public string? Version { get; set; }
        
        [JsonPropertyName("buildDate")]
        public string? BuildDate { get; set; }
    }
}
