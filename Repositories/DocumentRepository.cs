using Microsoft.EntityFrameworkCore;
using CQCDMS.Data;
using CQCDMS.Models;

namespace CQCDMS.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DmsDbContext _context;
        private readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository(DmsDbContext context, ILogger<DocumentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _context.Documents.OrderByDescending(d => d.UploadDate).ToListAsync();
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<Document> AddAsync(Document document)
        {
            document.UploadDate = DateTime.Now;
            document.DateCreated = DateTime.Now;
            
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateAsync(Document document, bool clearFile = false)
        {
            var existingDocument = await _context.Documents.FindAsync(document.Id) ?? throw new ArgumentException("Document not found");

            // Update fields
            existingDocument.Name = document.Name;
            existingDocument.FaxNumber = document.FaxNumber;
            existingDocument.Sender = document.Sender;
            existingDocument.Recipient = document.Recipient;
            existingDocument.Status = document.Status;
            existingDocument.FaxType = document.FaxType;
            existingDocument.NumberOfPages = document.NumberOfPages;
            existingDocument.Notes = document.Notes;
            existingDocument.IsImportant = document.IsImportant;
            existingDocument.CommitmentDate = document.CommitmentDate;

            // Update file info if provided
            if (clearFile)
            {
                existingDocument.FilePath = null;
                existingDocument.FileUrl = null;
                existingDocument.FileSize = null;
            }
            else if (!string.IsNullOrEmpty(document.FilePath))
            {
                existingDocument.FilePath = document.FilePath;
                existingDocument.FileUrl = document.FileUrl;
                existingDocument.FileSize = document.FileSize;
            }

            _context.Documents.Update(existingDocument);
            await _context.SaveChangesAsync();
            return existingDocument;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Document>> SearchAsync(string searchTerm, string searchType, string status, string faxType)
        {
            var query = _context.Documents.AsQueryable();

            _logger.LogInformation("Repository SearchAsync called with: searchTerm='{SearchTerm}', searchType='{SearchType}', status='{Status}', faxType='{FaxType}'", 
                searchTerm, searchType, status, faxType);

            // Apply search filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                switch (searchType)
                {
                    case "name":
                        _logger.LogInformation("Searching in Name field for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => d.Name != null && d.Name.Contains(searchTerm));
                        break;
                    case "sender":
                        _logger.LogInformation("Searching in Sender field for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => d.Sender != null && d.Sender.Contains(searchTerm));
                        break;
                    case "recipient":
                        _logger.LogInformation("Searching in Recipient field for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => d.Recipient != null && d.Recipient.Contains(searchTerm));
                        break;
                    case "faxnumber":
                        _logger.LogInformation("Searching in FaxNumber field for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => d.FaxNumber != null && d.FaxNumber.Contains(searchTerm));
                        break;
                    case "faxtype":
                        _logger.LogInformation("Searching in FaxType field for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => d.FaxType != null && d.FaxType.Contains(searchTerm));
                        break;
                    case "notes":
                        _logger.LogInformation("Searching in Notes field for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => d.Notes != null && d.Notes.Contains(searchTerm));
                        break;
                    default: // "all"
                        _logger.LogInformation("Searching in all fields for term: {SearchTerm}", searchTerm);
                        query = query.Where(d => 
                            (d.Name != null && d.Name.Contains(searchTerm)) ||
                            (d.Sender != null && d.Sender.Contains(searchTerm)) ||
                            (d.Recipient != null && d.Recipient.Contains(searchTerm)) ||
                            (d.FaxNumber != null && d.FaxNumber.Contains(searchTerm)) ||
                            (d.FaxType != null && d.FaxType.Contains(searchTerm)) ||
                            (d.Notes != null && d.Notes.Contains(searchTerm)));
                        break;
                }
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(d => d.Status == status);
            }

            // Apply fax type filter
            if (!string.IsNullOrEmpty(faxType) && faxType != "all")
            {
                query = query.Where(d => d.FaxType == faxType);
            }

            return await query.OrderByDescending(d => d.UploadDate).ToListAsync();
        }

        public async Task<DocumentStatistics> GetStatisticsAsync()
        {
            return new DocumentStatistics
            {
                Total = await _context.Documents.CountAsync(),
                Sent = await _context.Documents.CountAsync(d => d.Status == "sent"),
                Received = await _context.Documents.CountAsync(d => d.Status == "received"),
            };
        }
    }
}