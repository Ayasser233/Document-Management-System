using CQCDMS.Models;
using CQCDMS.Repositories;

namespace CQCDMS.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly ILogger<DocumentService> _logger;
        private readonly IWebHostEnvironment _environment;

        public DocumentService(IDocumentRepository repository, ILogger<DocumentService> logger, IWebHostEnvironment environment)
        {
            _repository = repository;
            _logger = logger;
            _environment = environment;
        }

        public async Task<ServiceResult<IEnumerable<Document>>> GetAllDocumentsAsync()
        {
            try
            {
                var documents = await _repository.GetAllAsync();
                return new ServiceResult<IEnumerable<Document>>
                {
                    Success = true,
                    Data = documents,
                    Message = "تم جلب البيانات بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all documents");
                return new ServiceResult<IEnumerable<Document>>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب البيانات"
                };
            }
        }

        public async Task<ServiceResult<Document>> GetDocumentByIdAsync(int id)
        {
            try
            {
                var document = await _repository.GetByIdAsync(id);
                if (document == null)
                {
                    return new ServiceResult<Document>
                    {
                        Success = false,
                        Message = "الفاكس غير موجود"
                    };
                }

                return new ServiceResult<Document>
                {
                    Success = true,
                    Data = document,
                    Message = "تم جلب البيانات بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document {Id}", id);
                return new ServiceResult<Document>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب بيانات الفاكس"
                };
            }
        }

        public async Task<ServiceResult<Document>> CreateDocumentAsync(Document document, IFormFile? file)
        {
            try
            {

                // Add business logic validation here
                if (string.IsNullOrEmpty(document.Name))
                {
                    return new ServiceResult<Document>
                    {
                        Success = false,
                        Message = "اسم الوثيقة مطلوب"
                    };
                }

                // Additional validation for other required fields
                if (string.IsNullOrEmpty(document.Sender))
                {
                    return new ServiceResult<Document>
                    {
                        Success = false,
                        Message = "اسم المرسل مطلوب"
                    };
                }

                if (string.IsNullOrEmpty(document.Recipient))
                {
                    return new ServiceResult<Document>
                    {
                        Success = false,
                        Message = "اسم المستقبل مطلوب"
                    };
                }

                // Handle file upload
                if (file != null && file.Length > 0)
                {
                    var fileResult = await SaveFileAsync(file);
                    if (!fileResult.Success)
                    {
                        return new ServiceResult<Document>
                        {
                            Success = false,
                            Message = fileResult.Message
                        };
                    }

                    document.FilePath = fileResult.Data!.FilePath;
                    document.FileUrl = fileResult.Data.FileUrl;
                    document.FileSize = fileResult.Data.FileSize;
                }
                else
                {
                    _logger.LogInformation("No file uploaded or file is empty");
                }

                var createdDocument = await _repository.AddAsync(document);
                
                return new ServiceResult<Document>
                {
                    Success = true,
                    Data = createdDocument,
                    Message = "تم إضافة الفاكس بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document");
                return new ServiceResult<Document>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء إضافة الفاكس"
                };
            }
        }

        public async Task<ServiceResult<Document>> UpdateDocumentAsync(Document document, IFormFile? file, bool removeFile = false)
        {
            try
            {
                var existingDocument = await _repository.GetByIdAsync(document.Id);
                if (existingDocument == null)
                {
                    return new ServiceResult<Document>
                    {
                        Success = false,
                        Message = "الفاكس غير موجود"
                    };
                }

                // Audit: track changes
                _logger.LogInformation("Updating fax {Id}. Original values => Name:{OldName}, Sender:{OldSender}, Recipient:{OldRecipient}, Status:{OldStatus}, FaxType:{OldFaxType}, Pages:{OldPages}, Important:{OldImp}, CommitmentDate:{OldCommit}, HasFile:{HasFile}",
                    existingDocument.Id,
                    existingDocument.Name,
                    existingDocument.Sender,
                    existingDocument.Recipient,
                    existingDocument.Status,
                    existingDocument.FaxType,
                    existingDocument.NumberOfPages,
                    existingDocument.IsImportant,
                    existingDocument.CommitmentDate,
                    string.IsNullOrEmpty(existingDocument.FilePath) ? "No" : "Yes");

                // Handle new file upload
                if (file != null && file.Length > 0)
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(existingDocument.FilePath))
                    {
                        await DeleteFileAsync(existingDocument.FilePath);
                    }

                    var fileResult = await SaveFileAsync(file);
                    if (!fileResult.Success)
                    {
                        return new ServiceResult<Document>
                        {
                            Success = false,
                            Message = fileResult.Message
                        };
                    }

                    document.FilePath = fileResult.Data!.FilePath;
                    document.FileUrl = fileResult.Data.FileUrl;
                    document.FileSize = fileResult.Data.FileSize;
                }

                // If removeFile requested and no new file uploaded
                if (removeFile && (file == null || file.Length == 0) && !string.IsNullOrEmpty(existingDocument.FilePath))
                {
                    await DeleteFileAsync(existingDocument.FilePath);
                    document.FilePath = null;
                    document.FileUrl = null;
                    document.FileSize = null;
                }

                var updatedDocument = await _repository.UpdateAsync(document, removeFile && (file == null || file.Length == 0));

                _logger.LogInformation("Updated fax {Id}. New values => Name:{NewName}, Sender:{NewSender}, Recipient:{NewRecipient}, Status:{NewStatus}, FaxType:{NewFaxType}, Pages:{NewPages}, Important:{NewImp}, CommitmentDate:{NewCommit}, HasFile:{HasFile}",
                    updatedDocument.Id,
                    updatedDocument.Name,
                    updatedDocument.Sender,
                    updatedDocument.Recipient,
                    updatedDocument.Status,
                    updatedDocument.FaxType,
                    updatedDocument.NumberOfPages,
                    updatedDocument.IsImportant,
                    updatedDocument.CommitmentDate,
                    string.IsNullOrEmpty(updatedDocument.FilePath) ? "No" : "Yes");
                return new ServiceResult<Document>
                {
                    Success = true,
                    Data = updatedDocument,
                    Message = "تم تحديث الفاكس بنجاح"
                };
            }
            catch (ArgumentException)
            {
                return new ServiceResult<Document>
                {
                    Success = false,
                    Message = "الفاكس غير موجود"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document {Id}", document.Id);
                return new ServiceResult<Document>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء تحديث الفاكس"
                };
            }
        }

        public async Task<ServiceResult<bool>> DeleteDocumentAsync(int id)
        {
            try
            {
                var document = await _repository.GetByIdAsync(id);
                if (document == null)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        Message = "الفاكس غير موجود"
                    };
                }

                // Delete physical file if exists
                if (!string.IsNullOrEmpty(document.FilePath))
                {
                    await DeleteFileAsync(document.FilePath);
                }

                var result = await _repository.DeleteAsync(id);
                if (!result)
                {
                    return new ServiceResult<bool>
                    {
                        Success = false,
                        Message = "فشل في حذف الفاكس"
                    };
                }

                return new ServiceResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "تم حذف الفاكس بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {Id}", id);
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء حذف الفاكس"
                };
            }
        }

        public async Task<ServiceResult<IEnumerable<Document>>> SearchDocumentsAsync(string searchTerm, string searchType, string status, string faxType, string? dateFilter = null)
        {
            try
            {
                _logger.LogInformation("SearchDocumentsAsync called with: searchTerm='{SearchTerm}', searchType='{SearchType}', status='{Status}', faxType='{FaxType}', dateFilter='{DateFilter}'", 
                    searchTerm, searchType, status, faxType, dateFilter);
                    
                var documents = await _repository.SearchAsync(searchTerm, searchType, status, faxType);

                if (!string.IsNullOrWhiteSpace(dateFilter))
                {
                    var today = DateTime.Today;
                    switch (dateFilter.ToLower())
                    {
                        case "today":
                            documents = documents.Where(d => d.DateCreated.Date == today);
                            break;
                        case "week":
                            // بداية الأسبوع: نفترض آخر 7 أيام لتفادي اختلافات تعريف الأسبوع
                            var weekStart = today.AddDays(-6); // يشمل اليوم + ستة أيام سابقة
                            documents = documents.Where(d => d.DateCreated.Date >= weekStart && d.DateCreated.Date <= today);
                            break;
                        case "month":
                            documents = documents.Where(d => d.DateCreated.Year == today.Year && d.DateCreated.Month == today.Month);
                            break;
                        case "year":
                            documents = documents.Where(d => d.DateCreated.Year == today.Year);
                            break;
                    }
                }

                // ترتيب تنازلي حسب التاريخ
                documents = documents.OrderByDescending(d => d.DateCreated);
                return new ServiceResult<IEnumerable<Document>>
                {
                    Success = true,
                    Data = documents,
                    Message = $"تم العثور على {documents.Count()} نتيجة"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching documents");
                return new ServiceResult<IEnumerable<Document>>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء البحث"
                };
            }
        }

        public async Task<ServiceResult<DocumentStatistics>> GetStatisticsAsync()
        {
            try
            {
                var statistics = await _repository.GetStatisticsAsync();
                return new ServiceResult<DocumentStatistics>
                {
                    Success = true,
                    Data = statistics,
                    Message = "تم جلب الإحصائيات بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return new ServiceResult<DocumentStatistics>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب الإحصائيات"
                };
            }
        }

        public async Task<ServiceResult<byte[]>> DownloadDocumentAsync(int id)
        {
            try
            {
                var document = await _repository.GetByIdAsync(id);
                if (document == null || string.IsNullOrEmpty(document.FilePath))
                {
                    return new ServiceResult<byte[]>
                    {
                        Success = false,
                        Message = "الملف غير موجود"
                    };
                }

                // Resolve physical path (we now store relative uploads path). If FilePath is absolute keep it.
                var storedPath = document.FilePath;
                string physicalPath = storedPath!;
                if (!Path.IsPathRooted(storedPath))
                {
                    physicalPath = Path.Combine(_environment.WebRootPath, storedPath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                }

                if (!File.Exists(physicalPath))
                {
                    return new ServiceResult<byte[]>
                    {
                        Success = false,
                        Message = "الملف غير موجود على الخادم"
                    };
                }

                var fileBytes = await File.ReadAllBytesAsync(physicalPath);
                return new ServiceResult<byte[]>
                {
                    Success = true,
                    Data = fileBytes,
                    Message = "تم تحميل الملف بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {Id}", id);
                return new ServiceResult<byte[]>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء تحميل الملف"
                };
            }
        }

        private async Task<ServiceResult<FileInfo>> SaveFileAsync(IFormFile file)
        {
            try
            {
                // Determine root for storage. Prefer WebRoot; when running published offline it still points inside the publish directory.
                // Allow override using OFFLINE_STORAGE_ROOT (absolute) if set.
                var overrideRoot = Environment.GetEnvironmentVariable("OFFLINE_STORAGE_ROOT");
                string root = !string.IsNullOrWhiteSpace(overrideRoot) && Path.IsPathRooted(overrideRoot)
                    ? overrideRoot
                    : _environment.WebRootPath;

                var relativeFolder = Path.Combine("uploads", "faxes");
                var physicalFolder = Path.Combine(root, relativeFolder);
                Directory.CreateDirectory(physicalFolder);

                var safeOriginalName = Path.GetFileName(file.FileName); // mitigate path traversal
                var uniqueFileName = $"{Guid.NewGuid()}_{safeOriginalName}";
                var physicalPath = Path.Combine(physicalFolder, uniqueFileName);

                using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return new ServiceResult<FileInfo>
                {
                    Success = true,
                    Data = new FileInfo
                    {
                        // Persist relative path so published folder can relocate with whole tree
                        FilePath = Path.Combine(relativeFolder, uniqueFileName).Replace('\\', '/'),
                        FileUrl = $"/uploads/faxes/{uniqueFileName}",
                        FileSize = file.Length
                    },
                    Message = "تم حفظ الملف بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                return new ServiceResult<FileInfo>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء حفظ الملف"
                };
            }
        }

        private async Task DeleteFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            }
        }

        private class FileInfo
        {
            public string FilePath { get; set; } = string.Empty;
            public string FileUrl { get; set; } = string.Empty;
            public long FileSize { get; set; }
        }
    }
}