using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Data.Models
{
    public class WaiverDetails
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        public bool IsSigned { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? PdfUrl { get; set; }
        public string? QrCodeContent { get; set; }
        
        // Specific waiver content fields
        public bool TermsAccepted { get; set; }
        public bool InsuranceTermsAccepted { get; set; }
        public string? SignatureMetadata { get; set; } // IP, Device info, etc.
    }
}
