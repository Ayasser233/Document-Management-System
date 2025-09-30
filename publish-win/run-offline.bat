@echo off
echo Starting CQCDMS in Offline Mode...
set OFFLINE_ASSETS=true
set ASPNETCORE_ENVIRONMENT=Production
CQCDMS.exe
pause