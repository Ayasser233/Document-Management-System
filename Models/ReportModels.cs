namespace CQCDMS.Models
{
    public class DailyReportData
    {
        public DateTime Date { get; set; }
        public int TotalDocuments { get; set; }
        public int SentDocuments { get; set; }
        public int ReceivedDocuments { get; set; }
        public IEnumerable<Document> Documents { get; set; } = new List<Document>();
        public IEnumerable<TypeStatistics> TypeBreakdown { get; set; } = new List<TypeStatistics>();
    }

    public class MonthlyReportData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int TotalDocuments { get; set; }
        public int SentDocuments { get; set; }
        public int ReceivedDocuments { get; set; }
        public IEnumerable<DailyStatistics> DailyBreakdown { get; set; } = new List<DailyStatistics>();
        public IEnumerable<TypeStatistics> TypeBreakdown { get; set; } = new List<TypeStatistics>();
        public IEnumerable<SenderStatistics> SenderBreakdown { get; set; } = new List<SenderStatistics>();
        public double DailyAverage { get; set; }
        public int HighestDay { get; set; }
    }

    public class StatusReportData
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalDocuments { get; set; }
        public int SentDocuments { get; set; }
        public int ReceivedDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public double SentPercentage { get; set; }
        public double ReceivedPercentage { get; set; }
        public double PendingPercentage { get; set; }
    }

    public class SenderStatistics
    {
        public string SenderName { get; set; } = string.Empty;
        public int DocumentCount { get; set; }
        public int SentCount { get; set; }
        public int ReceivedCount { get; set; }
        public DateTime LastDocumentDate { get; set; }
        public double Percentage { get; set; }
    }

    public class TypeStatistics
    {
        public string FaxType { get; set; } = string.Empty;
        public string FaxTypeName { get; set; } = string.Empty;
        public int DocumentCount { get; set; }
        public double Percentage { get; set; }
        public string BadgeClass { get; set; } = string.Empty;
    }

    public class DailyStatistics
    {
        public DateTime Date { get; set; }
        public int TotalDocuments { get; set; }
        public int SentDocuments { get; set; }
        public int ReceivedDocuments { get; set; }
    }

    public class ReportsViewModel
    {
        public int TotalDocuments { get; set; }
        public int SentDocuments { get; set; }
        public int ReceivedDocuments { get; set; }
        public DailyReportData? TodayReport { get; set; }
        public MonthlyReportData? CurrentMonthReport { get; set; }
        public StatusReportData? StatusReport { get; set; }
        public IEnumerable<SenderStatistics> TopSenders { get; set; } = new List<SenderStatistics>();
        public IEnumerable<TypeStatistics> TypeBreakdown { get; set; } = new List<TypeStatistics>();
    }

    // Custom Report Models
    public class CustomReportRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? FaxType { get; set; }
        public string? Sender { get; set; }
        public string? Recipient { get; set; }
        public string ReportType { get; set; } = "summary"; // summary, detailed, statistics
        public string Format { get; set; } = "json"; // json, pdf, excel
    }

    public class CustomReportData
    {
        public string ReportTitle { get; set; } = string.Empty;
        public DateTime GeneratedDate { get; set; }
        public CustomReportRequest Parameters { get; set; } = new();
        public IEnumerable<Document> Documents { get; set; } = new List<Document>();
        public ReportSummary Summary { get; set; } = new();
        public IEnumerable<TypeStatistics> TypeBreakdown { get; set; } = new List<TypeStatistics>();
        public IEnumerable<SenderStatistics> SenderBreakdown { get; set; } = new List<SenderStatistics>();
    }

    public class ReportSummary
    {
        public int TotalDocuments { get; set; }
        public int SentDocuments { get; set; }
        public int ReceivedDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public double SentPercentage { get; set; }
        public double ReceivedPercentage { get; set; }
        public double PendingPercentage { get; set; }
        public DateTime? EarliestDate { get; set; }
        public DateTime? LatestDate { get; set; }
        public int UniqueSenders { get; set; }
        public int UniqueRecipients { get; set; }
    }

    // Chart Data Models
    public class ChartData
    {
        public string ChartType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IEnumerable<string> Labels { get; set; } = new List<string>();
        public IEnumerable<ChartDataset> Datasets { get; set; } = new List<ChartDataset>();
    }

    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public IEnumerable<double> Data { get; set; } = new List<double>();
        public string BackgroundColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public IEnumerable<string>? BackgroundColors { get; set; }
        public IEnumerable<string>? BorderColors { get; set; }
    }

    // Export Models
    public class ExportParameters
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AdditionalFilters { get; set; }
    }
}
