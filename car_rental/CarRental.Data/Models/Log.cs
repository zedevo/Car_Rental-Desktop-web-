using System;

namespace CarRental.Data.Models
{
    public class Log
    {
        public int Id { get; set; }
        
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
