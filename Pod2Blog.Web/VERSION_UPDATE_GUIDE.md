# Version Update Guide

## How to release a new version:

**It's automatic!** 🎉

Every time you push to the `main` branch, the GitHub Actions workflow will:
1. **Automatically generate** `version.json` with the commit SHA (first 7 chars) as the version
2. **Set the build date** to the current UTC timestamp
3. **Build and deploy** the application

**What happens next:**
- Users with the app open will be checked for updates every 5 minutes
- When a new version is detected (different commit SHA), they'll see a friendly update prompt
- They can reload immediately or dismiss and reload later
- The reload clears all caches to ensure fresh content

## How it works:

- **version.json**: Cache-busted JSON file with version info
- **VersionService**: Checks for updates periodically (every 5 minutes)
- **UpdateNotification**: Shows a modal overlay when update is available
- **Cache Control**: Headers prevent version.json from being cached
- **Hard Reload**: Clears service workers and caches before reloading

## Cache busting:

- `version.json` includes cache-control headers via `staticwebapp.config.json`
- The service adds a timestamp query parameter (`?t=...`) when fetching
- On reload, all caches and service workers are cleared
- Blazor's own cache-busting for framework files still works normally

## Testing:

1. Deploy the current version to GitHub Pages
2. Make a small change and push to main
3. Wait for GitHub Actions to deploy the new version
4. In the running app, wait up to 5 minutes
5. The update notification should appear with the new commit SHA

## Local Testing:

If you want to test the update notification locally:
1. Run the app: `dotnet run`
2. Note the version in `version.json` (e.g., "1.0.0")
3. While the app is running, edit `wwwroot/version.json` and change the version (e.g., "1.0.1")
4. Wait up to 5 minutes
5. The update notification should appear
