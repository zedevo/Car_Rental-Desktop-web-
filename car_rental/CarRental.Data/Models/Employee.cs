using System.ComponentModel.DataAnnotations;
using CarRental.Data.Enums;

namespace CarRental.Data.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        
        public string? IdCard { get; set; }
        
        public int Age { get; set; }
        
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }

        // Simple password storage for demo (In prod, use Hash)
        public string? Password { get; set; }
        
        public EmployeeRole Role { get; set; }
        
        // Driver specific
        public DriverStatus? DriverStatus { get; set; }
        
        // Desk/General status
        public bool IsClockedIn { get; set; }
        public DateTime? LastClockIn { get; set; }
        public DateTime? LastClockOut { get; set; }
        
        // Leave/Sickness
        public bool IsOnLeave { get; set; }
        public bool IsSick { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
    }
}
