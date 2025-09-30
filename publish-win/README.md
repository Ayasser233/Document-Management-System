# CQCDMS Windows Executable

## 🎯 Quick Start (Choose One Method)

### Method 1: Double-click the Batch File (Easiest)
1. Double-click `run-offline.bat`
2. The application will start automatically
3. Your web browser will open to http://localhost:5000

### Method 2: PowerShell Script
1. Right-click `run-offline.ps1` 
2. Select "Run with PowerShell"

### Method 3: Manual Command Line
1. Open Command Prompt in this folder
2. Run:
```cmd
set OFFLINE_ASSETS=true
set ASPNETCORE_ENVIRONMENT=Production
CQCDMS.exe
```

## 📁 What's Included

- **CQCDMS.exe** - Main application (self-contained, no .NET required)
- **wwwroot/** - Web assets (CSS, JavaScript, images, fonts)
- **run-offline.bat** - Easy startup script for Windows
- **run-offline.ps1** - PowerShell startup script
- **appsettings.json** - Application configuration

## 🌐 Application Access

After starting:
- **URL**: http://localhost:5000
- **Auto-open**: Browser should open automatically
- **Manual**: If browser doesn't open, navigate to the URL above

## 💾 File Storage

- **Uploaded Files**: `wwwroot/uploads/faxes/`
- **Database**: SQLite database will be created automatically
- **Logs**: Application logs in the console window

## ⚙️ Features

- **Offline Ready**: All assets included, no internet required
- **Self-Contained**: No .NET installation needed
- **Portable**: Copy entire folder to any Windows machine
- **Arabic Support**: Full RTL and Arabic text support
- **File Management**: Upload, view, search, and manage fax documents

## 🛠️ Troubleshooting

### Icons Not Showing
- Ensure `OFFLINE_ASSETS=true` environment variable is set
- Check that `wwwroot` folder exists next to CQCDMS.exe

### Port Already in Use
- Close other applications using port 5000
- Or modify `appsettings.json` to use a different port

### Database Issues
- The app uses SQLite by default (no setup required)
- Database file will be created automatically on first run

### Performance
- First startup may take a few seconds
- Subsequent startups are faster

## 📋 System Requirements

- **OS**: Windows 7 SP1+ / Windows 8.1+ / Windows 10+
- **Architecture**: x64 (64-bit)
- **RAM**: 2GB minimum, 4GB recommended
- **Disk**: 100MB free space
- **Browser**: Chrome, Firefox, Edge, or Safari

## 🔒 Security Notes

- Application runs locally on your machine
- No data sent to external servers
- All files stored locally in the application folder

## 📞 Support

For technical support or questions about the CQCDMS system, refer to the main project documentation or contact your system administrator.

---
**Version**: Self-contained Windows executable
**Built**: $(Get-Date -Format 'yyyy-MM-dd HH:mm')