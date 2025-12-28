using System.ComponentModel.DataAnnotations;
using CarRental.Data.Enums;

namespace CarRental.Data.Models
{
    public class Car
    {
        public int Id { get; set; }
        
        [Required]
        public string Make { get; set; } = string.Empty;
        
        [Required]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        public string PlateNumber { get; set; } = string.Empty;
        
        public CarType Type { get; set; }
        public CarStatus Status { get; set; } = CarStatus.Available;
        
        public decimal DailyRate { get; set; }

        public int Year { get; set; }
        
        [Required]
        public string Category { get; set; } = "Standard";
        
        public string? ImageUrl { get; set; }
        
        public bool IsAvailable { get; set; } = true;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
