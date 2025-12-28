using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using CarRental.Desktop.Models;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarRental.Desktop.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        [ObservableProperty] private int _totalCars;
        [ObservableProperty] private int _availableCars;
        [ObservableProperty] private int _activeRentals;
        [ObservableProperty] private int _pendingOrders;
        
        [ObservableProperty]
        private ObservableCollection<WorkEvent> _calendarEvents = new();
        
        public DashboardViewModel()
        {
            Console.WriteLine("[DEBUG] DashboardViewModel initialized.");
            LoadData();
        }

        [RelayCommand]
        public void LoadData()
        {
            CalendarEvents.Clear();
            TotalCars = 0; AvailableCars = 0; PendingOrders = 0;

            try {
                using var context = CreateDbContext();
                
                // Stats
                TotalCars = context.Cars.Count();
                AvailableCars = context.Cars.Count(c => c.IsAvailable);
                ActiveRentals = context.Orders.Count(o => o.Status == OrderStatus.Active);
                PendingOrders = context.Orders.Count(o => o.Status == OrderStatus.Pending);
                
                // Work Calendar Population
                var events = new List<WorkEvent>();

                // 1. Pending Pickups
                var pendingOrders = context.Orders.Include(o => o.Client)
                    .Where(o => o.Status == OrderStatus.Pending)
                    .OrderBy(o => o.StartDate)
                    .Take(5).ToList();
                foreach (var po in pendingOrders)
                {
                    events.Add(new WorkEvent { 
                        Title = "New Request", 
                        Description = $"{po.Client?.Name} - {po.RequestedCarType} @ {po.PickupLocation}",
                        Time = po.StartDate,
                        Type = "Pickup",
                        Status = "Pending"
                    });
                }

                // 2. Active Returns
                var activeOrders = context.Orders.Include(o => o.Client).Include(o => o.Car)
                    .Where(o => o.Status == OrderStatus.Active)
                    .OrderBy(o => o.EndDate)
                    .Take(5).ToList();
                foreach (var ao in activeOrders)
                {
                    events.Add(new WorkEvent {
                        Title = "Return Due",
                        Description = $"{ao.Client?.Name} returning {ao.Car?.Model ?? "Car"}",
                        Time = ao.EndDate,
                        Type = "Return",
                        Status = "In-Progress"
                    });
                }

                // 3. Maintenance/Expenses Alerts
                var alerts = context.Expenses
                    .Where(e => e.ExpiryDate != null && e.ExpiryDate > DateTime.Now)
                    .OrderBy(e => e.ExpiryDate)
                    .Take(5).ToList();
                foreach (var alert in alerts)
                {
                    events.Add(new WorkEvent {
                        Title = alert.Type.ToString(),
                        Description = alert.Description,
                        Time = alert.ExpiryDate!.Value,
                        Type = "Maintenance",
                        Status = "Upcoming"
                    });
                }

                CalendarEvents = new ObservableCollection<WorkEvent>(events.OrderBy(e => e.Time));

            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] Dashboard LoadData failed: {ex.Message}");
                // Fallback / Mock can be added here if needed
            }
        }
    }
}
