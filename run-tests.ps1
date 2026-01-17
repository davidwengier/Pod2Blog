#!/usr/bin/env pwsh
# Script to run Playwright tests with dev server

Write-Host "Starting Pod2Blog dev server..." -ForegroundColor Cyan

# Start the dev server in background
$serverJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    dotnet run --project Pod2Blog.Web/Pod2Blog.Web.csproj --urls="http://localhost:5000"
}

Write-Host "Waiting for server to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

try {
    # Test if server is responding
    $maxAttempts = 10
    $attempt = 0
    $serverReady = $false
    
    while ($attempt -lt $maxAttempts -and -not $serverReady) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000" -TimeoutSec 2 -UseBasicParsing
            if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 404) {
                $serverReady = $true
                Write-Host "✓ Server is ready!" -ForegroundColor Green
            }
        }
        catch {
            $attempt++
            Write-Host "Waiting for server... (attempt $attempt/$maxAttempts)" -ForegroundColor Yellow
            Start-Sleep -Seconds 2
        }
    }
    
    if (-not $serverReady) {
        Write-Host "❌ Server failed to start within timeout" -ForegroundColor Red
        exit 1
    }
    
    # Run the tests
    Write-Host "`nRunning Playwright tests..." -ForegroundColor Cyan
    dotnet test Pod2Blog.Tests/Pod2Blog.Tests.csproj --logger "console;verbosity=detailed"
    $testExitCode = $LASTEXITCODE
    
    if ($testExitCode -eq 0) {
        Write-Host "`n✓ All tests passed!" -ForegroundColor Green
    } else {
        Write-Host "`n❌ Some tests failed" -ForegroundColor Red
    }
    
    exit $testExitCode
}
finally {
    # Stop the server
    Write-Host "`nStopping dev server..." -ForegroundColor Yellow
    Stop-Job $serverJob
    Remove-Job $serverJob
}
