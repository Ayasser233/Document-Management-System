using Microsoft.AspNetCore.Mvc;
using CQCDMS.Models;
using CQCDMS.Services;

namespace CQCDMS.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IReportService _reportService;

        public ReportsController(ILogger<ReportsController> logger, IReportService reportService)
        {
            _logger = logger;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await _reportService.GetReportsViewModelAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reports data");
                return View(new ReportsViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDailyReport(DateTime date)
        {
            var result = await _reportService.GetDailyReportAsync(date);
            if (result.Success && result.Data != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        date = result.Data.Date.ToString("yyyy-MM-dd"),
                        dateArabic = result.Data.Date.ToString("dd/MM/yyyy"),
                        totalDocuments = result.Data.TotalDocuments,
                        sentDocuments = result.Data.SentDocuments,
                        receivedDocuments = result.Data.ReceivedDocuments,
                        typeBreakdown = result.Data.TypeBreakdown.Select(t => new
                        {
                            type = t.FaxTypeName,
                            count = t.DocumentCount,
                            percentage = t.Percentage,
                            badgeClass = t.BadgeClass
                        })
                    }
                });
            }
            return Json(new { success = false, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> GetMonthlyReport(int year, int month)
        {
            var result = await _reportService.GetMonthlyReportAsync(year, month);
            if (result.Success && result.Data != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        year = result.Data.Year,
                        month = result.Data.Month,
                        monthName = result.Data.MonthName,
                        totalDocuments = result.Data.TotalDocuments,
                        sentDocuments = result.Data.SentDocuments,
                        receivedDocuments = result.Data.ReceivedDocuments,
                        dailyAverage = result.Data.DailyAverage,
                        highestDay = result.Data.HighestDay,
                        dailyBreakdown = result.Data.DailyBreakdown.Select(d => new
                        {
                            date = d.Date.ToString("yyyy-MM-dd"),
                            day = d.Date.Day,
                            totalDocuments = d.TotalDocuments,
                            sentDocuments = d.SentDocuments,
                            receivedDocuments = d.ReceivedDocuments
                        }),
                        senderBreakdown = result.Data.SenderBreakdown.Take(10).Select(s => new
                        {
                            senderName = s.SenderName,
                            documentCount = s.DocumentCount,
                            percentage = s.Percentage
                        })
                    }
                });
            }
            return Json(new { success = false, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> GetStatusReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = await _reportService.GetStatusReportAsync(startDate, endDate);
            if (result.Success && result.Data != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        startDate = result.Data.StartDate?.ToString("yyyy-MM-dd"),
                        endDate = result.Data.EndDate?.ToString("yyyy-MM-dd"),
                        totalDocuments = result.Data.TotalDocuments,
                        sentDocuments = result.Data.SentDocuments,
                        receivedDocuments = result.Data.ReceivedDocuments,
                        pendingDocuments = result.Data.PendingDocuments,
                        sentPercentage = result.Data.SentPercentage,
                        receivedPercentage = result.Data.ReceivedPercentage,
                        pendingPercentage = result.Data.PendingPercentage
                    }
                });
            }
            return Json(new { success = false, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> GetSenderStatistics()
        {
            var result = await _reportService.GetSenderStatisticsAsync();
            if (result.Success && result.Data != null)
            {
                return Json(new
                {
                    success = true,
                    data = result.Data.Select(s => new
                    {
                        senderName = s.SenderName,
                        documentCount = s.DocumentCount,
                        sentCount = s.SentCount,
                        receivedCount = s.ReceivedCount,
                        lastDocumentDate = s.LastDocumentDate.ToString("yyyy-MM-dd"),
                        percentage = s.Percentage
                    })
                });
            }
            return Json(new { success = false, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> GetTypeStatistics()
        {
            var result = await _reportService.GetTypeStatisticsAsync();
            if (result.Success && result.Data != null)
            {
                return Json(new
                {
                    success = true,
                    data = result.Data.Select(t => new
                    {
                        faxType = t.FaxType,
                        faxTypeName = t.FaxTypeName,
                        documentCount = t.DocumentCount,
                        percentage = t.Percentage,
                        badgeClass = t.BadgeClass
                    })
                });
            }
            return Json(new { success = false, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCustomReport([FromBody] CustomReportRequest request)
        {
            var result = await _reportService.GenerateCustomReportAsync(request);
            if (result.Success && result.Data != null)
            {
                return Json(new
                {
                    success = true,
                    data = result.Data
                });
            }
            return Json(new { success = false, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(string reportType, string parameters = "")
        {
            try
            {
                var result = await _reportService.ExportReportToPdfAsync(reportType, parameters);
                if (result.Success && result.Data != null)
                {
                    return File(result.Data, "application/pdf", $"تقرير_{reportType}_{DateTime.Now:yyyyMMdd}.pdf");
                }
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to PDF");
                return BadRequest("حدث خطأ في تصدير التقرير");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string reportType, string parameters = "")
        {
            try
            {
                var result = await _reportService.ExportReportToExcelAsync(reportType, parameters);
                if (result.Success && result.Data != null)
                {
                    return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        $"تقرير_{reportType}_{DateTime.Now:yyyyMMdd}.xlsx");
                }
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to Excel");
                return BadRequest("حدث خطأ في تصدير التقرير");
            }
        }
    }
}
