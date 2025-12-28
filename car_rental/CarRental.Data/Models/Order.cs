using System;
using CarRental.Data.Enums;

namespace CarRental.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        
        public int? CarId { get; set; }
        public Car? Car { get; set; }
        
        public int? DriverId { get; set; }
        public Employee? Driver { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        
        public CarType RequestedCarType { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public decimal TotalAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        
        public int NumberOfPassengers { get; set; }
        
        public WaiverDetails? Waiver { get; set; }
    }
}
