using System;

namespace CarRental.Desktop.Models
{
    public class WorkEvent
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Type { get; set; } = "General"; // Pickup, Return, Maintenance, Alert
        public string Status { get; set; } = "Pending";
        
        public string DisplayTime => Time.ToString("HH:mm");
        public string DisplayDate => Time.ToString("MMM dd");
        
        public string Icon => Type switch
        {
            "Pickup" => "🚗",
            "Return" => "🅿️",
            "Maintenance" => "🔧",
            "Alert" => "⚠️",
            _ => "📅"
        };
        
        public string Color => Type switch
        {
            "Pickup" => "#4CAF50",
            "Return" => "#2196F3",
            "Maintenance" => "#FF9800",
            "Alert" => "#F44336",
            _ => "#FFFFFF"
        };
    }
}
