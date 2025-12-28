using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.ViewModels
{
    public partial class OperationsViewModel : ViewModelBase
    {
        [ObservableProperty] private ObservableCollection<Order> _pendingOrders = new();
        [ObservableProperty] private ObservableCollection<Order> _activeOrders = new();
        [ObservableProperty] private ObservableCollection<Car> _availableCars = new();
        [ObservableProperty] private ObservableCollection<Employee> _availableDrivers = new();

        [ObservableProperty] private Order? _selectedPendingOrder;
        [ObservableProperty] private Order? _selectedActiveOrder;
        [ObservableProperty] private Car? _selectedCar;
        [ObservableProperty] private Employee? _selectedDriver;

        public OperationsViewModel()
        {
            LoadOperationsData();
        }

        [RelayCommand]
        public void LoadOperationsData()
        {
            using var context = CreateDbContext();
            
            PendingOrders = new ObservableCollection<Order>(
                context.Orders.Include(o => o.Client)
                .Where(o => o.Status == OrderStatus.Pending).ToList());
                
            ActiveOrders = new ObservableCollection<Order>(
                context.Orders.Include(o => o.Client).Include(o => o.Driver).Include(o => o.Car)
                .Where(o => o.Status == OrderStatus.Active).ToList());
                
            AvailableCars = new ObservableCollection<Car>(
                context.Cars.Where(c => c.IsAvailable).ToList());
                
            AvailableDrivers = new ObservableCollection<Employee>(
                context.Employees.Where(e => e.Role == EmployeeRole.Driver && e.DriverStatus == DriverStatus.Available).ToList());
        }

        [RelayCommand]
        private void AssignOrder()
        {
            if (SelectedPendingOrder == null || SelectedCar == null || SelectedDriver == null) return;

            using var context = CreateDbContext();
            var order = context.Orders.Find(SelectedPendingOrder.Id);
            if (order != null)
            {
                order.CarId = SelectedCar.Id;
                order.DriverId = SelectedDriver.Id;
                order.Status = OrderStatus.Active;
                
                var driver = context.Employees.Find(SelectedDriver.Id);
                if (driver != null) driver.DriverStatus = DriverStatus.Busy;
                
                var car = context.Cars.Find(SelectedCar.Id);
                if (car != null) car.IsAvailable = false; 

                // Audit Log
                context.Logs.Add(new Log { 
                    EmployeeId = ViewModelBase.CurrentUser?.Id,
                    Action = $"Assigned {car?.PlateNumber} and Driver {driver?.Name} to Order {order.Id}",
                    Timestamp = DateTime.UtcNow
                });

                context.SaveChanges();
                LoadOperationsData();
                SelectedPendingOrder = null;
            }
        }

        [RelayCommand]
        private void CompleteOrder()
        {
             if (SelectedActiveOrder == null) return;
             
             using var context = CreateDbContext();
             var order = context.Orders.Find(SelectedActiveOrder.Id);
             if (order != null)
             {
                 order.Status = OrderStatus.Completed;
                 if (order.DriverId != null)
                 {
                     var driver = context.Employees.Find(order.DriverId);
                     if (driver != null) driver.DriverStatus = DriverStatus.Available;
                 }
                 if (order.CarId != null)
                 {
                     var car = context.Cars.Find(order.CarId);
                     if (car != null) car.IsAvailable = true;
                 }

                 context.Logs.Add(new Log { 
                    EmployeeId = ViewModelBase.CurrentUser?.Id,
                    Action = $"Completed Mission for Order {order.Id}",
                    Timestamp = DateTime.UtcNow
                 });

                 context.SaveChanges();
                 LoadOperationsData();
                 SelectedActiveOrder = null;
             }
        }
    }
}
