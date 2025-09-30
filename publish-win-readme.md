# CQCDMS Offline Deployment

This folder contains the self-contained Windows executable for CQCDMS with offline asset support.

## Quick Start

### Option 1: Using Batch File (Easiest)
Double-click `run-offline.bat`

### Option 2: Using PowerShell
Right-click `run-offline.ps1` → "Run with PowerShell"

### Option 3: Manual Command Line
```cmd
set OFFLINE_ASSETS=true
CQCDMS.exe
```

## Files Included

- `CQCDMS.exe` - Main application executable
- `wwwroot/` - Static web assets (CSS, JS, fonts, images)
  - `lib/local/bootstrap-icons.css` - Bootstrap icons for offline use
  - `lib/local/fonts/` - Font files for icons
  - `lib/local/cairo.css` - Arabic font support
- `run-offline.bat` - Windows batch script to start in offline mode
- `run-offline.ps1` - PowerShell script to start in offline mode

## Environment Variables

- `OFFLINE_ASSETS=true` - Forces the app to use local CSS/font files instead of CDN
- `ASPNETCORE_ENVIRONMENT=Production` - Sets production environment

## File Storage

When running offline, uploaded fax files are stored in:
`wwwroot/uploads/faxes/`

This folder will be created automatically when you upload your first file.

## Database

The app will use MySQL if available, or fall back to SQLite for offline use.
SQLite database file: `Data/dms.db`

## Troubleshooting

1. **Icons not showing**: Ensure `OFFLINE_ASSETS=true` is set
2. **404 errors**: Check that `wwwroot` folder exists next to the exe
3. **Database errors**: Ensure MySQL is running or let it fall back to SQLite

## Port

Default port: http://localhost:5000
The app will open automatically in your default browser.