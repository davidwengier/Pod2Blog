# Pod2Blog 🎙️ → 📝

Transform your ideas into blog posts through an interactive podcast-style interview with AI.

**100% Client-Side** - Runs entirely in your browser as a WebAssembly application. Your API keys never leave your device!

## Features

- **Interactive Podcast Interface**: Have a natural conversation with an AI interviewer about your blog topic
- **Real-time Speech Recognition**: Uses the Web Speech API for browser-based voice recording and transcription
- **AI-Powered Content Generation**: 
  - AI generates thoughtful interview questions based on your topic
  - Automatically converts your conversation into a polished blog post
- **GitHub Models Integration**: Powered by GitHub's AI models
- **Privacy First**: All configuration stored locally in your browser - no server-side storage
- **Static Hosting**: Deploy to GitHub Pages, Netlify, or any static host

## Quick Start

### 1. Run the Application

```bash
dotnet run
```

Navigate to `http://localhost:5172` (or the URL shown in your terminal).

### 2. Create a GitHub Token (30 seconds)

1. Click **Settings**
2. Click the **"Create GitHub Token"** button (opens GitHub in a new tab)
3. On GitHub, click **"Generate token"** at the bottom
4. Copy the token and paste it back in the Settings page
5. Select your preferred AI model (default: `gpt-4o-mini`)
6. Click **Save Configuration**

> **Note:** No permissions needed! GitHub Models works with any authenticated token.

### 3. Create Your First Blog Post

1. Return to **Home**
2. Enter a topic you want to write about
3. Click **Start Interview**
4. Allow microphone access when prompted
5. Answer the AI's questions naturally
6. Click **Finish Interview** when done
7. Review and download your generated blog post!

## GitHub Models Setup

GitHub Models is **free for developers** and requires only a GitHub account:

1. Visit the Settings page in the app
2. Click **"Create GitHub Token"** - this opens a pre-filled form on GitHub
3. Click **"Generate token"** on GitHub (no permissions/scopes needed!)
4. Copy the token back to Pod2Blog
5. Choose your model (recommended: `gpt-4o-mini` - fast & free)

**Available models:**
- `gpt-4o-mini` - Fast, efficient (recommended)
- `gpt-4o` - More powerful
- `o1-mini`, `o1-preview` - Advanced reasoning
- And many more on [GitHub Models](https://github.com/marketplace/models)

## Browser Compatibility

### Speech Recognition Support:
- ✅ Chrome/Edge (recommended)
- ✅ Safari
- ❌ Firefox (limited speech API support)

### WebAssembly Support:
- ✅ All modern browsers (Chrome, Edge, Firefox, Safari)

## Deployment

Since this is a static Blazor WebAssembly app, you can deploy it anywhere:

### GitHub Pages (Automated)

**Automatic deployment on every push to main:**

1. Enable GitHub Pages in your repository settings:
   - Go to **Settings** → **Pages**
   - Source: Deploy from a branch
   - Branch: `gh-pages` / `root`
   - Click **Save**

2. Update the base path in `.github/workflows/deploy-gh-pages.yml`:
   - Replace `Pod2Blog` with your repository name in the `sed` command (line 28)

3. Push to main branch - the workflow will automatically:
   - Build your Blazor WASM app
   - Publish to the `gh-pages` branch
   - Deploy to `https://yourusername.github.io/your-repo-name`

**Manual deployment:**
```bash
dotnet publish -c Release
# Upload contents of bin/Release/net10.0/publish/wwwroot to GitHub Pages
# Remember to add a .nojekyll file and set the correct base href in index.html
```

### Netlify / Vercel
```bash
dotnet publish -c Release
# Deploy the wwwroot folder
```

### Azure Static Web Apps
```bash
dotnet publish -c Release
# Use Azure CLI or portal to deploy
```

## Architecture

### Client-Side Only
- **Blazor WebAssembly** - Runs entirely in the browser
- **Local Storage** - API keys and settings stored locally
- **Direct API Calls** - Browser makes API calls directly to GitHub Models
- **No Backend Required** - Can be hosted as static files

### Version Detection & Auto-Update
The app includes automatic version detection that prompts users (especially on mobile) when a new version is available:

- **Automatic Versioning**: GitHub Actions automatically generates version from commit SHA
- **Periodic Checks**: Checks for updates every 5 minutes
- **Cache-Busting**: `version.json` is never cached, ensuring fresh version info
- **Update Prompt**: Shows a friendly modal when an update is available
- **Hard Reload**: Clears all caches and service workers before reloading
- **Mobile Optimized**: UI designed for mobile devices with responsive design

**Version updates are fully automatic** - just push to main and GitHub Actions handles everything!

See [VERSION_UPDATE_GUIDE.md](Pod2Blog.Web/VERSION_UPDATE_GUIDE.md) for details.

### Key Components

- **`/Components/Pages/Home.razor`**: Topic selection and interview start
- **`/Components/Pages/Settings.razor`**: AI service configuration UI
- **`/Components/Pages/Interview.razor`**: Interactive podcast recording interface
- **`/Components/Pages/BlogPreview.razor`**: Generated blog post preview and export
- **`/Services/IAIService.cs`**: Interface for AI service providers
- **`/Services/OpenAIService.cs`**: GitHub Models API implementation
- **`/Services/LocalStorageService.cs`**: Browser local storage wrapper
- **`/wwwroot/pod2blog.js`**: Browser speech recognition and utilities


## Privacy & Security

- ✅ Tokens stored in browser local storage only
- ✅ No server-side storage or logging
- ✅ Direct API calls from browser to GitHub Models
- ✅ Clear configuration data anytime from Settings
- ⚠️ Use HTTPS in production to protect tokens in transit

## Future Enhancements

- [ ] Persist interview sessions to IndexedDB
- [ ] Support for multiple languages
- [ ] Export to various formats (HTML, PDF, Word)
- [ ] Edit and refine generated blog posts
- [ ] Voice customization for AI interviewer
- [ ] Offline support with service workers
- [ ] Analytics on writing topics

## Development

Built with:
- .NET 10.0
- Blazor WebAssembly
- Web Speech API
- Bootstrap 5
- Browser Local Storage API

### Project Structure

```
Pod2Blog/
├── Pod2Blog.slnx          # Visual Studio solution file
├── Pod2Blog.Web/          # Blazor WebAssembly application
│   ├── Components/        # Razor components
│   ├── Models/            # Data models
│   ├── Services/          # Business logic
│   └── wwwroot/           # Static assets
└── Pod2Blog.Tests/        # Playwright browser tests
    └── InterviewTests.cs
```

### Running Tests

Automated browser tests using Playwright and xUnit:

```bash
# Install Playwright browsers (one-time setup)
playwright install

# Run tests with dev server
./run-tests.ps1

# Or manually
cd Pod2Blog.Web
dotnet run --urls="http://localhost:5000"  # In one terminal
cd ..
dotnet test Pod2Blog.Tests/Pod2Blog.Tests.csproj    # In another terminal
```

Tests verify:
- Navigation between pages
- Settings page PAT configuration UI
- Text input mode fallback
- Interview flow UI elements

## License

MIT License - feel free to use and modify for your needs!
