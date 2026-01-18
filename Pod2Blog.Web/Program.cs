using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Pod2Blog.Web;
using Pod2Blog.Web.Services;
using Pod2Blog.Web.Services.Audio;
using Pod2Blog.Web.Services.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HTTP client for AI services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAIService, OpenAIService>();
builder.Services.AddScoped<IVersionService, VersionService>();
builder.Services.AddScoped<IAudioService, AudioService>();
builder.Services.AddScoped<IClipboardService, ClipboardService>();
builder.Services.AddScoped<IFileDownloadService, FileDownloadService>();

await builder.Build().RunAsync();
