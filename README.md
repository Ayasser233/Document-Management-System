# CQCDMS - Document Management System

CQCDMS is a modern, Arabic-first Document Management System (DMS) built with ASP.NET Core 9.0 and Entity Framework Core. It is designed for organizations that require robust document archiving, search, reporting, and workflow management, with a focus on right-to-left (RTL) Arabic interfaces and advanced reporting features.

## Features

- 📄 **Document Archiving**: Store, categorize, and manage scanned or digital documents with metadata.
- 🔍 **Advanced Search**: Powerful search and filtering by subject, sender, recipient, status, department, and date.
- 📊 **Comprehensive Reports**: Daily, monthly, sender, and status-based reports with export options (PDF/Excel).
- 🏷️ **Customizable Departments**: Fully Arabic department/branch names, easily configurable.
- 📥 **File Upload & Download**: Secure file storage, download, and access control.
- 🛡️ **Role-based Access**: (Planned) User roles and permissions for secure document access.
- 🖥️ **Modern UI**: Responsive Bootstrap 5 interface, RTL support, and Arabic terminology throughout.

## Technology Stack

- **Backend**: ASP.NET Core 9.0 (MVC), Entity Framework Core
- **Database**: MySQL
- **Frontend**: Razor Views, Bootstrap 5, jQuery
- **Reporting**: Custom service layer, extensible for PDF/Excel export
- **Authentication**: (Planned) ASP.NET Identity

## Project Structure

```
CQCDMS/
├── Controllers/           # MVC Controllers (Home, Reports, etc.)
├── Models/                # Data models, ViewModels, constants
├── Services/              # Business logic, report generation, file handling
├── Views/                 # Razor views (Home, Reports, Shared, etc.)
├── wwwroot/               # Static files (css, js, uploads, libraries)
├── Properties/            # Launch settings
├── appsettings.json       # Configuration
├── Program.cs             # App entry point & DI setup
├── CQCDMS.csproj          # Project file
└── ...
```

## Key Files & Folders

- `Controllers/ReportsController.cs` — Dedicated controller for all report endpoints
- `Services/ReportService.cs` — Business logic for generating reports
- `Models/ReportModels.cs` — Data structures for reports and charts
- `Models/ReportConstants.cs` — Centralized Arabic department/status mappings
- `Views/Reports/Index.cshtml` — Main reports dashboard
- `Views/Home/Search.cshtml` — Advanced document search
- `Views/Home/Management.cshtml` — Document management (add/edit/delete)
- `wwwroot/uploads/faxes/` — Uploaded document files

## How to Run

1. **Clone the repository**
2. **Configure the database**: Set your MySQL connection string in `appsettings.json`
3. **Build & run**:
   ```sh
   dotnet build
   dotnet run
   ```
4. **Access the app**: Open [http://localhost:5000](http://localhost:5000) in your browser

## Customization

- **Departments/Branches**: Update `Models/ReportConstants.cs` to change department names (Arabic supported)
- **Reports**: Extend `ReportService.cs` and `ReportsController.cs` for new report types
- **File Storage**: Files are stored in `wwwroot/uploads/faxes/` by default

## Documentation

- `REPORTS_ARCHITECTURE.md` — Detailed architecture and report system documentation
- `DEPARTMENT_MIGRATION_GUIDE.md` — Guide for updating department/branch names and migrating data

## License

This project is licensed under the MIT License.

---

For questions, suggestions, or contributions, please open an issue or pull request on GitHub.
