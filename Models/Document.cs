using System;
using System.ComponentModel.DataAnnotations;

namespace CQCDMS.Models
{
    public class Document
    {
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; }
        
        public string? FaxNumber { get; set; }
        
        public string? Sender { get; set; }
        
        public string? Recipient { get; set; }
        
        public string? Status { get; set; }
        
        public string? FaxType { get; set; } // e.g., "planning_training_operations", "needs_technical_affairs", "intelligence_modern_systems", etc.
        
        public int? NumberOfPages { get; set; }
        
        public string? Notes { get; set; }
        
        public string? FilePath { get; set; } // Add this for file URL/path
        
        public string? FileUrl { get; set; } // Add this for accessible file URL
        
        public long? FileSize { get; set; } // Add this for file size in bytes
        
        public DateTime DateCreated { get; set; } = DateTime.Now;
                
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
