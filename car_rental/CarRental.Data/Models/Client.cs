using System.ComponentModel.DataAnnotations;
using CarRental.Data.Enums;

namespace CarRental.Data.Models
{
    public class Client
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty; // Name of Org or Person
        
        public ClientType Type { get; set; }
        
        public string ContactInfo { get; set; } = string.Empty; // Phone or Email
        
        // Organization specific
        public string? RegistrationNumber { get; set; } 
        public int? PassengerCount { get; set; }
        
        // Individual specific
        public string? NationalId { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public string? CreditCardNumber { get; set; } // Demo only
    }
}
