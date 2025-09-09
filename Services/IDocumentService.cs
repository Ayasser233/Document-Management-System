using CQCDMS.Models;
using CQCDMS.Repositories;

namespace CQCDMS.Services
{
    public interface IDocumentService
    {
        Task<ServiceResult<IEnumerable<Document>>> GetAllDocumentsAsync();
        Task<ServiceResult<Document>> GetDocumentByIdAsync(int id);
        Task<ServiceResult<Document>> CreateDocumentAsync(Document document, IFormFile? file);
        Task<ServiceResult<Document>> UpdateDocumentAsync(Document document, IFormFile? file);
        Task<ServiceResult<bool>> DeleteDocumentAsync(int id);
        Task<ServiceResult<IEnumerable<Document>>> SearchDocumentsAsync(string searchTerm, string searchType, string status, string faxType, string? dateFilter = null);
        Task<ServiceResult<DocumentStatistics>> GetStatisticsAsync();
        Task<ServiceResult<byte[]>> DownloadDocumentAsync(int id);
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}