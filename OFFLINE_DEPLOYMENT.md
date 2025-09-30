# Offline Deployment Guide

Set environment variables before launching for full offline mode.

Windows PowerShell example:

```
$env:OFFLINE_SQLITE="true"      # Use embedded SQLite database file (data/dms.db)
$env:OFFLINE_ASSETS="true"      # Use local fonts and icons
```

Then run:
```
./CQCDMS.exe --urls http://localhost:8080
```

## First Run (SQLite)
The database file will be created automatically. Run EF migrations manually if needed (initial code-first create occurs on first access by default). If explicit migration application is required, temporarily run using `dotnet ef database update` with the same OFFLINE_SQLITE variable set.

## Migrating Existing MySQL Data to SQLite (Optional)
1. Export MySQL data to CSV or SQL.
2. Use a tool (e.g., DB Browser for SQLite) to import into `data/dms.db`.
3. Ensure schema matches current migrations.

## Reverting to MySQL
Unset `OFFLINE_SQLITE` or set it to anything other than `true` and the app will use the `MySqlConnection` from configuration.

## Assets
To fully replace Bootstrap Icons copy actual font files into `wwwroot/lib/local/` and update `bootstrap-icons.css` with proper `@font-face` `src:` paths.
