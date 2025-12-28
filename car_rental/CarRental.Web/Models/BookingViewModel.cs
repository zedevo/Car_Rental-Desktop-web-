using System;
using System.ComponentModel.DataAnnotations;
using CarRental.Data.Enums;

namespace CarRental.Web.Models
{
    public class BookingViewModel
    {
        // Client Info
        [Required]
        public string ClientName { get; set; } = string.Empty;
        
        [Required]
        public string ContactInfo { get; set; } = string.Empty;
        
        [Required]
        public ClientType ClientType { get; set; }
        
        // Organization Specific
        public string? RegistrationNumber { get; set; }
        public int? PassengerCount { get; set; }
        public decimal? SecurityDeposit { get; set; } // Just for display/input
        
        // Individual Specific
        public string? NationalId { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public string? CreditCardNumber { get; set; }

        // Order Info
        [Required]
        public CarType SelectedCarType { get; set; }
        
        [Required]
        public DateTime PickupDate { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        
        [Required]
        public string PickupLocation { get; set; } = string.Empty;
        
        [Required]
        public string Destination { get; set; } = string.Empty;
    }
}
