using CQCDMS.Models;

namespace CQCDMS.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document?> GetByIdAsync(int id);
        Task<Document> AddAsync(Document document);
        Task<Document> UpdateAsync(Document document);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Document>> SearchAsync(string searchTerm, string searchType, string status, string faxType);
        Task<DocumentStatistics> GetStatisticsAsync();
    }

    public class DocumentStatistics
    {
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Received { get; set; }
    }
}