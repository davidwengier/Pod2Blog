using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Pod2Blog;
using Pod2Blog.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HTTP client for AI services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAIService, OpenAIService>();

await builder.Build().RunAsync();
