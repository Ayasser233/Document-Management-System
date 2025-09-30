using CQCDMS.Models;

namespace CQCDMS.Services
{
    public interface IReportService
    {
        // Main report generation methods
        Task<ServiceResult<DailyReportData>> GetDailyReportAsync(DateTime date);
        Task<ServiceResult<MonthlyReportData>> GetMonthlyReportAsync(int year, int month);
        Task<ServiceResult<StatusReportData>> GetStatusReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ServiceResult<IEnumerable<SenderStatistics>>> GetSenderStatisticsAsync();
        Task<ServiceResult<IEnumerable<TypeStatistics>>> GetTypeStatisticsAsync();
        
        // Custom reports
        Task<ServiceResult<CustomReportData>> GenerateCustomReportAsync(CustomReportRequest request);
        
        // Export functions
        Task<ServiceResult<byte[]>> ExportReportToPdfAsync(string reportType, string parameters);
        Task<ServiceResult<byte[]>> ExportReportToExcelAsync(string reportType, string parameters);
        
        // View model preparation
        Task<ReportsViewModel> GetReportsViewModelAsync();
        
        // Chart data
        Task<ServiceResult<ChartData>> GetChartDataAsync(ChartType chartType, DateTime? startDate = null, DateTime? endDate = null);
    }

    public enum ChartType
    {
        DailyTrend,
        MonthlyTrend,
        StatusDistribution,
        TypeDistribution,
        SenderDistribution
    }
}
