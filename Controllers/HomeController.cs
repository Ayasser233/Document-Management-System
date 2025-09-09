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

    [HttpPost]
    public async Task<IActionResult> AddDocument([FromForm] Document document, IFormFile? file)
    {
        var result = await _documentService.CreateDocumentAsync(document, file);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetDocument(int id)
    {
        var result = await _documentService.GetDocumentByIdAsync(id);
        if (result.Success && result.Data != null)
        {
            return Json(new
            {
                success = true,
                data = new
                {
                    id = result.Data.Id,
                    name = result.Data.Name,
                    faxNumber = result.Data.FaxNumber,
                    sender = result.Data.Sender,
                    recipient = result.Data.Recipient,
                    status = result.Data.Status,
                    faxType = result.Data.FaxType,
                    numberOfPages = result.Data.NumberOfPages,
                    notes = result.Data.Notes,
                    fileUrl = result.Data.FileUrl,
                    fileSize = result.Data.FileSize,
                    uploadDate = result.Data.UploadDate.ToString("dd/MM/yyyy HH:mm")
                }
            });
        }
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDocument([FromForm] Document document, IFormFile? file)
    {
        var result = await _documentService.UpdateDocumentAsync(document, file);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
