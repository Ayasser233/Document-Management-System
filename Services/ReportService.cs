using CQCDMS.Models;
using CQCDMS.Services;

namespace CQCDMS.Services
{
    public class ReportService : IReportService
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IDocumentService documentService, ILogger<ReportService> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<ServiceResult<DailyReportData>> GetDailyReportAsync(DateTime date)
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<DailyReportData>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var dailyDocs = documentsResult.Data.Where(d => d.DateCreated.Date == date.Date).ToList();

                var report = new DailyReportData
                {
                    Date = date,
                    TotalDocuments = dailyDocs.Count,
                    SentDocuments = dailyDocs.Count(d => d.Status?.ToLower() == "sent"),
                    ReceivedDocuments = dailyDocs.Count(d => d.Status?.ToLower() == "received"),
                    Documents = dailyDocs,
                    TypeBreakdown = GetTypeStatistics(dailyDocs)
                };

                return new ServiceResult<DailyReportData>
                {
                    Success = true,
                    Data = report,
                    Message = $"تم إنشاء التقرير اليومي لتاريخ {date:yyyy/MM/dd}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating daily report for date {Date}", date);
                return new ServiceResult<DailyReportData>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء التقرير اليومي"
                };
            }
        }

        public async Task<ServiceResult<MonthlyReportData>> GetMonthlyReportAsync(int year, int month)
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<MonthlyReportData>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var monthlyDocs = documentsResult.Data.Where(d => d.DateCreated.Year == year && d.DateCreated.Month == month).ToList();

                var dailyBreakdown = GetDailyBreakdown(monthlyDocs, year, month);
                var dailyAverage = monthlyDocs.Count > 0 ? (double)monthlyDocs.Count / DateTime.DaysInMonth(year, month) : 0;
                var highestDay = dailyBreakdown.Any() ? dailyBreakdown.Max(d => d.TotalDocuments) : 0;

                var report = new MonthlyReportData
                {
                    Year = year,
                    Month = month,
                    MonthName = GetMonthName(month),
                    TotalDocuments = monthlyDocs.Count,
                    SentDocuments = monthlyDocs.Count(d => d.Status?.ToLower() == "sent"),
                    ReceivedDocuments = monthlyDocs.Count(d => d.Status?.ToLower() == "received"),
                    DailyBreakdown = dailyBreakdown,
                    TypeBreakdown = GetTypeStatistics(monthlyDocs),
                    SenderBreakdown = GetSenderStatistics(monthlyDocs),
                    DailyAverage = Math.Round(dailyAverage, 1),
                    HighestDay = highestDay
                };

                return new ServiceResult<MonthlyReportData>
                {
                    Success = true,
                    Data = report,
                    Message = $"تم إنشاء التقرير الشهري لشهر {report.MonthName} {year}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating monthly report for {Year}/{Month}", year, month);
                return new ServiceResult<MonthlyReportData>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء التقرير الشهري"
                };
            }
        }

        public async Task<ServiceResult<StatusReportData>> GetStatusReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<StatusReportData>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var documents = documentsResult.Data.AsEnumerable();
                
                if (startDate.HasValue && endDate.HasValue)
                {
                    documents = documents.Where(d => d.DateCreated.Date >= startDate.Value.Date && 
                                               d.DateCreated.Date <= endDate.Value.Date);
                }

                var total = documents.Count();
                var sent = documents.Count(d => d.Status?.ToLower() == "sent");
                var received = documents.Count(d => d.Status?.ToLower() == "received");
                var pending = total - sent - received;

                var report = new StatusReportData
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalDocuments = total,
                    SentDocuments = sent,
                    ReceivedDocuments = received,
                    PendingDocuments = pending,
                    SentPercentage = total > 0 ? Math.Round((double)sent / total * 100, 1) : 0,
                    ReceivedPercentage = total > 0 ? Math.Round((double)received / total * 100, 1) : 0,
                    PendingPercentage = total > 0 ? Math.Round((double)pending / total * 100, 1) : 0
                };

                return new ServiceResult<StatusReportData>
                {
                    Success = true,
                    Data = report,
                    Message = "تم إنشاء تقرير الحالات"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating status report");
                return new ServiceResult<StatusReportData>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء تقرير الحالات"
                };
            }
        }

        public async Task<ServiceResult<IEnumerable<SenderStatistics>>> GetSenderStatisticsAsync()
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<IEnumerable<SenderStatistics>>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var senderStats = GetSenderStatistics(documentsResult.Data);

                return new ServiceResult<IEnumerable<SenderStatistics>>
                {
                    Success = true,
                    Data = senderStats,
                    Message = "تم إنشاء إحصائيات المرسلين"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sender statistics");
                return new ServiceResult<IEnumerable<SenderStatistics>>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء إحصائيات المرسلين"
                };
            }
        }

        public async Task<ServiceResult<IEnumerable<TypeStatistics>>> GetTypeStatisticsAsync()
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<IEnumerable<TypeStatistics>>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var typeStats = GetTypeStatistics(documentsResult.Data);

                return new ServiceResult<IEnumerable<TypeStatistics>>
                {
                    Success = true,
                    Data = typeStats,
                    Message = "تم إنشاء إحصائيات الأنواع"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating type statistics");
                return new ServiceResult<IEnumerable<TypeStatistics>>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء إحصائيات الأنواع"
                };
            }
        }

        public async Task<ServiceResult<CustomReportData>> GenerateCustomReportAsync(CustomReportRequest request)
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<CustomReportData>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var documents = ApplyFilters(documentsResult.Data, request);

                var summary = GenerateReportSummary(documents);
                var typeBreakdown = GetTypeStatistics(documents);
                var senderBreakdown = GetSenderStatistics(documents);

                var reportData = new CustomReportData
                {
                    ReportTitle = GenerateReportTitle(request),
                    GeneratedDate = DateTime.Now,
                    Parameters = request,
                    Documents = documents,
                    Summary = summary,
                    TypeBreakdown = typeBreakdown,
                    SenderBreakdown = senderBreakdown
                };

                return new ServiceResult<CustomReportData>
                {
                    Success = true,
                    Data = reportData,
                    Message = "تم إنشاء التقرير المخصص بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating custom report");
                return new ServiceResult<CustomReportData>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء التقرير المخصص"
                };
            }
        }

        public async Task<ServiceResult<byte[]>> ExportReportToPdfAsync(string reportType, string parameters)
        {
            try
            {
                // TODO: Implement PDF export using libraries like iTextSharp or PdfSharp
                // For now, return a placeholder
                await Task.Delay(100);
                
                return new ServiceResult<byte[]>
                {
                    Success = false,
                    Message = "تصدير PDF غير متاح حالياً"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to PDF");
                return new ServiceResult<byte[]>
                {
                    Success = false,
                    Message = "حدث خطأ في تصدير التقرير"
                };
            }
        }

        public async Task<ServiceResult<byte[]>> ExportReportToExcelAsync(string reportType, string parameters)
        {
            try
            {
                // TODO: Implement Excel export using libraries like EPPlus or ClosedXML
                // For now, return a placeholder
                await Task.Delay(100);
                
                return new ServiceResult<byte[]>
                {
                    Success = false,
                    Message = "تصدير Excel غير متاح حالياً"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report to Excel");
                return new ServiceResult<byte[]>
                {
                    Success = false,
                    Message = "حدث خطأ في تصدير التقرير"
                };
            }
        }

        public async Task<ReportsViewModel> GetReportsViewModelAsync()
        {
            try
            {
                var today = DateTime.Now;
                var currentMonth = today.Month;
                var currentYear = today.Year;

                // Get basic statistics
                var statisticsResult = await _documentService.GetStatisticsAsync();
                
                // Get today's report
                var dailyReportResult = await GetDailyReportAsync(today);
                
                // Get current month report
                var monthlyReportResult = await GetMonthlyReportAsync(currentYear, currentMonth);
                
                // Get status report
                var statusReportResult = await GetStatusReportAsync();
                
                // Get sender statistics (top 5)
                var senderStatsResult = await GetSenderStatisticsAsync();
                
                // Get type statistics
                var typeStatsResult = await GetTypeStatisticsAsync();

                return new ReportsViewModel
                {
                    TotalDocuments = statisticsResult.Success ? statisticsResult.Data?.Total ?? 0 : 0,
                    SentDocuments = statisticsResult.Success ? statisticsResult.Data?.Sent ?? 0 : 0,
                    ReceivedDocuments = statisticsResult.Success ? statisticsResult.Data?.Received ?? 0 : 0,
                    TodayReport = dailyReportResult.Success ? dailyReportResult.Data : null,
                    CurrentMonthReport = monthlyReportResult.Success ? monthlyReportResult.Data : null,
                    StatusReport = statusReportResult.Success ? statusReportResult.Data : null,
                    TopSenders = senderStatsResult.Success ? senderStatsResult.Data?.Take(5) ?? [] : [],
                    TypeBreakdown = typeStatsResult.Success ? typeStatsResult.Data ?? [] : []
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating reports view model");
                return new ReportsViewModel();
            }
        }

        public async Task<ServiceResult<ChartData>> GetChartDataAsync(ChartType chartType, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var documentsResult = await _documentService.GetAllDocumentsAsync();
                if (!documentsResult.Success || documentsResult.Data == null)
                {
                    return new ServiceResult<ChartData>
                    {
                        Success = false,
                        Message = "فشل في جلب البيانات"
                    };
                }

                var documents = documentsResult.Data.AsEnumerable();
                if (startDate.HasValue && endDate.HasValue)
                {
                    documents = documents.Where(d => d.DateCreated.Date >= startDate.Value.Date && 
                                               d.DateCreated.Date <= endDate.Value.Date);
                }

                var chartData = chartType switch
                {
                    ChartType.StatusDistribution => GenerateStatusDistributionChart(documents),
                    ChartType.TypeDistribution => GenerateTypeDistributionChart(documents),
                    ChartType.SenderDistribution => GenerateSenderDistributionChart(documents),
                    ChartType.DailyTrend => GenerateDailyTrendChart(documents, startDate, endDate),
                    ChartType.MonthlyTrend => GenerateMonthlyTrendChart(documents),
                    _ => new ChartData { Title = "نوع الرسم البياني غير مدعوم" }
                };

                return new ServiceResult<ChartData>
                {
                    Success = true,
                    Data = chartData,
                    Message = "تم إنشاء بيانات الرسم البياني"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating chart data for {ChartType}", chartType);
                return new ServiceResult<ChartData>
                {
                    Success = false,
                    Message = "حدث خطأ في إنشاء بيانات الرسم البياني"
                };
            }
        }

        #region Helper Methods

        private IEnumerable<SenderStatistics> GetSenderStatistics(IEnumerable<Document> documents)
        {
            var total = documents.Count();
            return documents
                .GroupBy(d => d.Sender)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g => new SenderStatistics
                {
                    SenderName = g.Key!,
                    DocumentCount = g.Count(),
                    SentCount = g.Count(d => d.Status?.ToLower() == "sent"),
                    ReceivedCount = g.Count(d => d.Status?.ToLower() == "received"),
                    LastDocumentDate = g.Max(d => d.DateCreated),
                    Percentage = total > 0 ? Math.Round((double)g.Count() / total * 100, 1) : 0
                })
                .OrderByDescending(s => s.DocumentCount)
                .ToList();
        }

        private IEnumerable<TypeStatistics> GetTypeStatistics(IEnumerable<Document> documents)
        {
            var total = documents.Count();
            var typeNames = ReportConstants.FaxTypeNames;
            var badgeClasses = ReportConstants.FaxTypeBadgeClasses;

            return documents
                .GroupBy(d => d.FaxType)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g => new TypeStatistics
                {
                    FaxType = g.Key!,
                    FaxTypeName = typeNames.ContainsKey(g.Key!) ? typeNames[g.Key!] : g.Key!,
                    DocumentCount = g.Count(),
                    Percentage = total > 0 ? Math.Round((double)g.Count() / total * 100, 1) : 0,
                    BadgeClass = badgeClasses.ContainsKey(g.Key!) ? badgeClasses[g.Key!] : "bg-secondary"
                })
                .OrderByDescending(t => t.DocumentCount)
                .ToList();
        }

        private IEnumerable<DailyStatistics> GetDailyBreakdown(IEnumerable<Document> documents, int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var dailyStats = new List<DailyStatistics>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day);
                var dayDocs = documents.Where(d => d.DateCreated.Date == date.Date).ToList();

                dailyStats.Add(new DailyStatistics
                {
                    Date = date,
                    TotalDocuments = dayDocs.Count,
                    SentDocuments = dayDocs.Count(d => d.Status?.ToLower() == "sent"),
                    ReceivedDocuments = dayDocs.Count(d => d.Status?.ToLower() == "received")
                });
            }

            return dailyStats;
        }

        private string GetMonthName(int month)
        {
            return ReportConstants.MonthNames.ContainsKey(month) ? ReportConstants.MonthNames[month] : "";
        }

        private IEnumerable<Document> ApplyFilters(IEnumerable<Document> documents, CustomReportRequest request)
        {
            var filteredDocs = documents;

            if (request.StartDate.HasValue)
                filteredDocs = filteredDocs.Where(d => d.DateCreated.Date >= request.StartDate.Value.Date);

            if (request.EndDate.HasValue)
                filteredDocs = filteredDocs.Where(d => d.DateCreated.Date <= request.EndDate.Value.Date);

            if (!string.IsNullOrEmpty(request.Status))
                filteredDocs = filteredDocs.Where(d => d.Status?.ToLower() == request.Status.ToLower());

            if (!string.IsNullOrEmpty(request.FaxType))
                filteredDocs = filteredDocs.Where(d => d.FaxType?.ToLower() == request.FaxType.ToLower());

            if (!string.IsNullOrEmpty(request.Sender))
                filteredDocs = filteredDocs.Where(d => d.Sender?.Contains(request.Sender, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrEmpty(request.Recipient))
                filteredDocs = filteredDocs.Where(d => d.Recipient?.Contains(request.Recipient, StringComparison.OrdinalIgnoreCase) == true);

            return filteredDocs.ToList();
        }

        private ReportSummary GenerateReportSummary(IEnumerable<Document> documents)
        {
            var docList = documents.ToList();
            var total = docList.Count;
            var sent = docList.Count(d => d.Status?.ToLower() == "sent");
            var received = docList.Count(d => d.Status?.ToLower() == "received");
            var pending = total - sent - received;

            return new ReportSummary
            {
                TotalDocuments = total,
                SentDocuments = sent,
                ReceivedDocuments = received,
                PendingDocuments = pending,
                SentPercentage = total > 0 ? Math.Round((double)sent / total * 100, 1) : 0,
                ReceivedPercentage = total > 0 ? Math.Round((double)received / total * 100, 1) : 0,
                PendingPercentage = total > 0 ? Math.Round((double)pending / total * 100, 1) : 0,
                EarliestDate = docList.Any() ? docList.Min(d => d.DateCreated) : null,
                LatestDate = docList.Any() ? docList.Max(d => d.DateCreated) : null,
                UniqueSenders = docList.Select(d => d.Sender).Where(s => !string.IsNullOrEmpty(s)).Distinct().Count(),
                UniqueRecipients = docList.Select(d => d.Recipient).Where(r => !string.IsNullOrEmpty(r)).Distinct().Count()
            };
        }

        private string GenerateReportTitle(CustomReportRequest request)
        {
            var title = "تقرير مخصص";
            
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                title += $" من {request.StartDate.Value:yyyy/MM/dd} إلى {request.EndDate.Value:yyyy/MM/dd}";
            }
            else if (request.StartDate.HasValue)
            {
                title += $" من {request.StartDate.Value:yyyy/MM/dd}";
            }
            else if (request.EndDate.HasValue)
            {
                title += $" حتى {request.EndDate.Value:yyyy/MM/dd}";
            }

            return title;
        }

        private ChartData GenerateStatusDistributionChart(IEnumerable<Document> documents)
        {
            var docList = documents.ToList();
            var sent = docList.Count(d => d.Status?.ToLower() == "sent");
            var received = docList.Count(d => d.Status?.ToLower() == "received");
            var pending = docList.Count - sent - received;

            return new ChartData
            {
                ChartType = "pie",
                Title = "توزيع الحالات",
                Labels = new[] { "مُرسل", "مُستقبل", "معلق" },
                Datasets = new[]
                {
                    new ChartDataset
                    {
                        Label = "الحالات",
                        Data = new double[] { sent, received, pending },
                        BackgroundColors = new[] { "#28a745", "#17a2b8", "#ffc107" },
                        BorderColors = new[] { "#1e7e34", "#138496", "#e0a800" }
                    }
                }
            };
        }

        private ChartData GenerateTypeDistributionChart(IEnumerable<Document> documents)
        {
            var typeStats = GetTypeStatistics(documents).Take(8).ToList();
            
            return new ChartData
            {
                ChartType = "doughnut",
                Title = "توزيع أنواع الفاكسات",
                Labels = typeStats.Select(t => t.FaxTypeName),
                Datasets = new[]
                {
                    new ChartDataset
                    {
                        Label = "الأنواع",
                        Data = typeStats.Select(t => (double)t.DocumentCount),
                        BackgroundColors = typeStats.Select(t => GetColorFromBadgeClass(t.BadgeClass))
                    }
                }
            };
        }

        private ChartData GenerateSenderDistributionChart(IEnumerable<Document> documents)
        {
            var senderStats = GetSenderStatistics(documents).Take(10).ToList();
            
            return new ChartData
            {
                ChartType = "bar",
                Title = "أكثر المرسلين نشاطاً",
                Labels = senderStats.Select(s => s.SenderName),
                Datasets = new[]
                {
                    new ChartDataset
                    {
                        Label = "عدد الفاكسات",
                        Data = senderStats.Select(s => (double)s.DocumentCount),
                        BackgroundColor = "#007bff",
                        BorderColor = "#0056b3"
                    }
                }
            };
        }

        private ChartData GenerateDailyTrendChart(IEnumerable<Document> documents, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? documents.Min(d => d.DateCreated).Date;
            var end = endDate ?? documents.Max(d => d.DateCreated).Date;
            
            var dailyData = new List<(DateTime Date, int Count)>();
            for (var date = start; date <= end; date = date.AddDays(1))
            {
                var count = documents.Count(d => d.DateCreated.Date == date);
                dailyData.Add((date, count));
            }

            return new ChartData
            {
                ChartType = "line",
                Title = "الاتجاه اليومي للفاكسات",
                Labels = dailyData.Select(d => d.Date.ToString("MM/dd")),
                Datasets = new[]
                {
                    new ChartDataset
                    {
                        Label = "عدد الفاكسات",
                        Data = dailyData.Select(d => (double)d.Count),
                        BackgroundColor = "#007bff",
                        BorderColor = "#0056b3"
                    }
                }
            };
        }

        private ChartData GenerateMonthlyTrendChart(IEnumerable<Document> documents)
        {
            var monthlyData = documents
                .GroupBy(d => new { d.DateCreated.Year, d.DateCreated.Month })
                .Select(g => new { 
                    Period = $"{g.Key.Year}/{g.Key.Month:00}", 
                    Count = g.Count() 
                })
                .OrderBy(x => x.Period)
                .ToList();

            return new ChartData
            {
                ChartType = "line",
                Title = "الاتجاه الشهري للفاكسات",
                Labels = monthlyData.Select(m => m.Period),
                Datasets = new[]
                {
                    new ChartDataset
                    {
                        Label = "عدد الفاكسات",
                        Data = monthlyData.Select(m => (double)m.Count),
                        BackgroundColor = "#28a745",
                        BorderColor = "#1e7e34"
                    }
                }
            };
        }

        private string GetColorFromBadgeClass(string badgeClass)
        {
            return badgeClass switch
            {
                "bg-primary" => "#007bff",
                "bg-secondary" => "#6c757d",
                "bg-success" => "#28a745",
                "bg-warning" => "#ffc107",
                "bg-danger" => "#dc3545",
                "bg-info" => "#17a2b8",
                "bg-dark" => "#343a40",
                "bg-light text-dark" => "#f8f9fa",
                _ => "#6c757d"
            };
        }

        #endregion
    }
}
