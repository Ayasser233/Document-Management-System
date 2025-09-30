# PowerShell script to publish with offline assets
# This script sets the environment variable and publishes the app for Windows offline use

Write-Host "Publishing CQCDMS for Windows with Offline Assets..." -ForegroundColor Green

# Set environment variables for the build
$env:OFFLINE_ASSETS = "true"
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Clean previous build
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
Remove-Item -Path ".\bin\Release" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".\publish-win" -Recurse -Force -ErrorAction SilentlyContinue

# Publish the application
Write-Host "Publishing application..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true -o ".\publish-win" /p:PublishSingleFile=true

# Check if publish was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Publish completed successfully!" -ForegroundColor Green
    Write-Host "📁 Output location: .\publish-win\" -ForegroundColor Cyan
    Write-Host "🚀 To run offline: Set OFFLINE_ASSETS=true and run CQCDMS.exe" -ForegroundColor Cyan
    
    # Create a batch file to run with offline assets
    $batchContent = @"
@echo off
echo Starting CQCDMS in Offline Mode...
set OFFLINE_ASSETS=true
set ASPNETCORE_ENVIRONMENT=Production
CQCDMS.exe
pause
"@
    
    $batchContent | Out-File -FilePath ".\publish-win\run-offline.bat" -Encoding ascii
    Write-Host "📝 Created run-offline.bat for easy offline execution" -ForegroundColor Green
    
    # Create a PowerShell script for offline execution
    $psContent = @"
# PowerShell script to run CQCDMS in offline mode
Write-Host "Starting CQCDMS in Offline Mode..." -ForegroundColor Green
`$env:OFFLINE_ASSETS = "true"
`$env:ASPNETCORE_ENVIRONMENT = "Production"
.\CQCDMS.exe
"@
    
    $psContent | Out-File -FilePath ".\publish-win\run-offline.ps1" -Encoding utf8
    Write-Host "📝 Created run-offline.ps1 for PowerShell execution" -ForegroundColor Green
    
    # Copy README
    Copy-Item ".\publish-win-readme.md" ".\publish-win\README.md" -Force
    Write-Host "📖 Copied README.md with usage instructions" -ForegroundColor Green
    
} else {
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor White
Write-Host "1. Navigate to .\publish-win\ folder" -ForegroundColor White  
Write-Host "2. Run 'run-offline.bat' or 'run-offline.ps1'" -ForegroundColor White
Write-Host "3. Or manually: set OFFLINE_ASSETS=true && CQCDMS.exe" -ForegroundColor White