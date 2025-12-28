using System;
using System.ComponentModel.DataAnnotations;
using CarRental.Data.Enums;

namespace CarRental.Data.Models
{
    public class Expense
    {
        public int Id { get; set; }
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public ExpenseType Type { get; set; }
        
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        
        // For alerts (e.g., Insurance expiry)
        public DateTime? ExpiryDate { get; set; }
        
        // Renewal Cycle (0 = One-time, 3, 6, 12 months)
        public int RenewalCycleMonths { get; set; }
        
        public int? CarId { get; set; }
        public Car? Car { get; set; }
    }
}
