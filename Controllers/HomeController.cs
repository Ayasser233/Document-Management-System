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
        var result = await _documentService.DownloadDocumentAsync(id);
        if (result.Success && result.Data != null)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            var fileName = document.Data?.Name + ".pdf";
            return File(result.Data, "application/octet-stream", fileName);
        }
        return NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Search(string searchTerm, string searchType, string status, string faxType)
    {
        var result = await _documentService.SearchDocumentsAsync(searchTerm, searchType, status, faxType);
        return View(result.Data ?? []);
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

    public IActionResult Reports()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
