using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CQCDMS.Models;
using CQCDMS.Services;

namespace CQCDMS.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDocumentService _documentService;

    public HomeController(ILogger<HomeController> logger, IDocumentService documentService)
    {
        _logger = logger;
        _documentService = documentService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Management()
    {
        var result = await _documentService.GetAllDocumentsAsync();
        return View(result.Data ?? []);
    }

    public async Task<IActionResult> Commitments()
    {
        var result = await _documentService.GetAllDocumentsAsync();
        return View(result.Data ?? []);
    }

    public IActionResult DebugEnv()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddDocument([FromForm] Document document, IFormFile? file)
    {
        var result = await _documentService.CreateDocumentAsync(document, file);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetDocument(int id)
    {
        try
        {
            _logger.LogInformation("Getting document with ID: {Id}", id);
            
            var result = await _documentService.GetDocumentByIdAsync(id);
            if (result.Success && result.Data != null)
            {
                var document = result.Data;
                
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = document.Id,
                        name = document.Name ?? "",
                        faxNumber = document.FaxNumber ?? "",
                        sender = document.Sender ?? "",
                        recipient = document.Recipient ?? "",
                        status = document.Status ?? "",
                        faxType = document.FaxType ?? "",
                        numberOfPages = document.NumberOfPages,
                        notes = document.Notes ?? "",
                        fileUrl = document.FileUrl ?? "",
                        fileSize = document.FileSize ?? 0,
                        uploadDate = document.UploadDate.ToString("dd/MM/yyyy HH:mm"),
                        dateCreated = document.DateCreated.ToString("dd/MM/yyyy HH:mm")
                    }
                });
            }
            
            _logger.LogWarning("Document not found or service failed. ID: {Id}, Success: {Success}, Message: {Message}", 
                id, result.Success, result.Message);
            
            return Json(new { success = false, message = result.Message ?? "لم يتم العثور على الفاكس" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document with ID: {Id}", id);
            return Json(new { success = false, message = "حدث خطأ في تحميل تفاصيل الفاكس" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDocument([FromForm] Document document, IFormFile? file, [FromForm] bool? removeFile)
    {
        var result = await _documentService.UpdateDocumentAsync(document, file, removeFile ?? false);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDocument([FromForm] int id)
    {
        // Log what we received
        _logger.LogInformation("Received DeleteDocument request for ID: {Id}", id);

        var result = await _documentService.DeleteDocumentAsync(id);
        return Json(new { success = result.Success, message = result.Message });
    }
    [HttpGet]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var document = await _documentService.GetDocumentByIdAsync(id);
        if (!document.Success || document.Data == null)
        {
            return NotFound("الوثيقة غير موجودة");
        }

        var result = await _documentService.DownloadDocumentAsync(id);
        if (result.Success && result.Data != null)
        {
            var doc = document.Data;
            
            // Determine file name and extension
            var fileName = doc.Name ?? "وثيقة";
            var fileExtension = "";
            
            // Try to get original file extension from FilePath
            if (!string.IsNullOrEmpty(doc.FilePath))
            {
                fileExtension = Path.GetExtension(doc.FilePath);
            }
            
            // If no extension found, default to .pdf
            if (string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = ".pdf";
            }
            
            // Sanitize file name for download
            var sanitizedFileName = fileName.Replace(" ", "_") + fileExtension;
            
            // Determine content type based on file extension
            var contentType = GetContentType(fileExtension);
            
            return File(result.Data, contentType, sanitizedFileName);
        }
        
        return NotFound(result.Message);
    }

    private string GetContentType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    [HttpPost]
    public async Task<IActionResult> Search(string searchTerm, string searchType, string status, string faxType, string? dateFilter = null)
    {
        var result = await _documentService.SearchDocumentsAsync(searchTerm, searchType, status, faxType, dateFilter);
        return Json(new { success = result.Success, data = result.Data, message = result.Message });
    }

    public async Task<IActionResult> Search()
    {
        var result = await _documentService.GetAllDocumentsAsync();
        return View(result.Data ?? []);
    }

    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _documentService.GetStatisticsAsync();
        if (result.Success && result.Data != null)
        {
            return Json(new
            {
                total = result.Data.Total,
                sent = result.Data.Sent,
                received = result.Data.Received,
            });
        }
        return Json(new { total = 0, sent = 0, received = 0 });
    }

    [HttpGet]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var result = await _documentService.GetAllDocumentsAsync();
            return Json(new 
            { 
                success = result.Success, 
                message = result.Message,
                count = result.Data?.Count() ?? 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database test failed");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DebugDocument(int id)
    {
        try
        {
            _logger.LogInformation("Debug: Getting document with ID: {Id}", id);
            
            // First check if the document exists in the repository
            var allDocs = await _documentService.GetAllDocumentsAsync();
            _logger.LogInformation("Debug: Total documents in database: {Count}", allDocs.Data?.Count() ?? 0);
            
            if (allDocs.Data != null)
            {
                var docExists = allDocs.Data.Any(d => d.Id == id);
                _logger.LogInformation("Debug: Document with ID {Id} exists: {Exists}", id, docExists);
                
                if (docExists)
                {
                    var doc = allDocs.Data.First(d => d.Id == id);
                    return Json(new
                    {
                        success = true,
                        message = "Document found via GetAll",
                        data = new
                        {
                            id = doc.Id,
                            name = doc.Name,
                            sender = doc.Sender,
                            recipient = doc.Recipient
                        }
                    });
                }
            }
            
            // Now try GetDocumentByIdAsync
            var result = await _documentService.GetDocumentByIdAsync(id);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data != null ? new { id = result.Data.Id, name = result.Data.Name } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Debug: Error getting document with ID: {Id}", id);
            return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
