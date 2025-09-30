# PowerShell script to run CQCDMS in offline mode
Write-Host "Starting CQCDMS in Offline Mode..." -ForegroundColor Green
$env:OFFLINE_ASSETS = "true"
$env:ASPNETCORE_ENVIRONMENT = "Production"
.\CQCDMS.exe