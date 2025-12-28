using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using System.Linq; // Added for ToList()
using System;

namespace CarRental.Desktop.ViewModels
{
    public partial class CarsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Car> _cars = new();

        [ObservableProperty]
        private Car? _selectedCar;
        
        // New Car Form
        [ObservableProperty] private string _newMake = string.Empty;
        [ObservableProperty] private string _newModel = string.Empty;
        [ObservableProperty] private string _newLicensePlate = string.Empty;
        [ObservableProperty] private int _newYear = 2024;
        [ObservableProperty] private decimal _newDailyRate = 50;

        public CarsViewModel()
        {
            LoadCars();
        }

        private void LoadCars()
        {
            try {
                using var context = CreateDbContext();
                var carsList = context.Cars.ToList();
                Cars = new ObservableCollection<Car>(carsList);
                Console.WriteLine($"[DEBUG] Loaded {carsList.Count} cars.");
                
                if (Cars.Count == 0)
                {
                    Console.WriteLine("[DEBUG] No cars in DB, using fallback mock.");
                    Cars.Add(new Car { Id = 1, Make = "Tesla", Model = "Model S", PlateNumber = "MOCK-01", DailyRate = 120 });
                    Cars.Add(new Car { Id = 2, Make = "Ford", Model = "F-150", PlateNumber = "MOCK-02", DailyRate = 85 });
                }
            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] LoadCars failed: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private void CreateCar()
        {
             if (string.IsNullOrWhiteSpace(NewMake) || string.IsNullOrWhiteSpace(NewModel)) return;
             
             var car = new Car
             {
                 Make = NewMake,
                 Model = NewModel,
                 PlateNumber = NewLicensePlate, // Mapped
                 Year = NewYear,
                 DailyRate = NewDailyRate,     // Mapped
                 IsAvailable = true,
                 ImageUrl = "https://placehold.co/600x400/png" // Placeholder
             };
             
             using var context = CreateDbContext();
             context.Cars.Add(car);
             context.SaveChanges();
             Cars.Add(car);
             
             // Reset
             NewMake = string.Empty; NewModel = string.Empty; NewLicensePlate = string.Empty;
        }

        [RelayCommand]
        private void UpdateCar()
        {
            if (SelectedCar == null) return;
            using var context = CreateDbContext();
            context.Cars.Update(SelectedCar);
            context.SaveChanges();
        }
        
        [RelayCommand]
        private void DeleteCar()
        {
             if (SelectedCar == null) return;
             using var context = CreateDbContext();
             context.Cars.Remove(SelectedCar);
             context.SaveChanges();
             Cars.Remove(SelectedCar);
             SelectedCar = null;
        }
    }
}
