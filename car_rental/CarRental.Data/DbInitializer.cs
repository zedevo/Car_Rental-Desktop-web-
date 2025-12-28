using CarRental.Data.Models;
using CarRental.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CarRental.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CarRentalDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Cars.Any())
            {
                // DB has been seeded, but we want to update images if they are missing or old placeholders
                UpdateImages(context);
                RemoveProblematicCars(context); // Explicitly remove cars as requested
                return;
            }

            var cars = new List<Car>
            {
                new Car
                {
                    Make = "Tesla",
                    Model = "Model S Plaid",
                    Year = 2024,
                    PlateNumber = "ELECTR-1",
                    Type = CarType.VIP,
                    Status = CarStatus.Available,
                    DailyRate = 299.00m,
                    Category = "Luxury",
                    ImageUrl = "https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&q=80&w=800"
                },
                new Car
                {
                    Make = "Porsche",
                    Model = "911 Carrera",
                    Year = 2023,
                    PlateNumber = "FAST-007",
                    Type = CarType.VIP,
                    Status = CarStatus.Available,
                    DailyRate = 450.00m,
                    Category = "Sport",
                    ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&q=80&w=800"
                },
                new Car
                {
                    Make = "Mercedes-Benz",
                    Model = "S-Class",
                    Year = 2024,
                    PlateNumber = "VIP-BOSS",
                    Type = CarType.VIP,
                    Status = CarStatus.Rented,
                    DailyRate = 350.00m,
                    Category = "VIP",
                    ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&q=80&w=800"
                },
                new Car
                {
                    Make = "Volvo",
                    Model = "9700",
                    Year = 2022,
                    PlateNumber = "TOUR-BUS",
                    Type = CarType.TouristBus,
                    Status = CarStatus.Available,
                    DailyRate = 600.00m,
                    Category = "Bus",
                    ImageUrl = "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?auto=format&fit=crop&q=80&w=800"
                },
                new Car
                {
                    Make = "Range Rover",
                    Model = "Autobiography",
                    Year = 2024,
                    PlateNumber = "LUX-SUV",
                    Type = CarType.VIP,
                    Status = CarStatus.Maintenance,
                    DailyRate = 400.00m,
                    Category = "Luxury",
                    ImageUrl = "https://images.unsplash.com/photo-1606220838315-056192d5e927?auto=format&fit=crop&q=80&w=800"
                }
            };

            context.Cars.AddRange(cars);
            context.SaveChanges();
        }

        private static void UpdateImages(CarRentalDbContext context)
        {
            Console.WriteLine("[DbInitializer] Running UpdateImages...");
            // Helper to update images for existing entities if they match known models
            var cars = context.Cars.ToList();
            bool changed = false;

            foreach (var car in cars)
            {
                Console.WriteLine($"[DbInitializer] Checking car: {car.Make} {car.Model}");
                if (car.Model.Contains("Model S")) {
                    car.ImageUrl = "https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&q=80&w=800";
                    changed = true;
                    Console.WriteLine("[DbInitializer] Updated Tesla");
                }
                else if (car.Model.Contains("911")) {
                    car.ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&q=80&w=800";
                    changed = true;
                    Console.WriteLine("[DbInitializer] Updated Porsche");
                }
                else if (car.Model.Contains("S-Class")) {
                    car.ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&q=80&w=800";
                    changed = true;
                    Console.WriteLine("[DbInitializer] Updated Mercedes");
                }
                 else if (car.Model.Contains("9700")) {
                    car.ImageUrl = "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?auto=format&fit=crop&q=80&w=800";
                    changed = true;
                    Console.WriteLine("[DbInitializer] Updated Volvo Bus");
                }
                else if (car.Model.Contains("Autobiography")) {
                    car.ImageUrl = "https://images.unsplash.com/photo-1606220838315-056192d5e927?auto=format&fit=crop&q=80&w=800";
                    changed = true;
                    Console.WriteLine("[DbInitializer] Updated Range Rover");
                }
                else if (car.Model.Contains("Mustang")) {
                    car.ImageUrl = "https://images.unsplash.com/photo-1494976388531-d1058494cdd8?auto=format&fit=crop&q=80&w=800";
                    changed = true;
                    Console.WriteLine("[DbInitializer] Updated Ford Mustang");
                }
            }

            if (changed)
            {
                context.SaveChanges();
                Console.WriteLine("[DbInitializer] Changes saved to DB.");
            }
            else 
            {
                Console.WriteLine("[DbInitializer] No changes needed.");
            }
        }

        private static void RemoveProblematicCars(CarRentalDbContext context)
        {
            var carsToRemove = context.Cars
                .Where(c => c.Model.Contains("M5") || c.Model.Contains("RS7") || c.Model.Contains("Camry") || c.Model.Contains("Urus"))
                .ToList();

            if (carsToRemove.Any())
            {
                Console.WriteLine($"[DbInitializer] Removing {carsToRemove.Count} cars with broken images (M5, RS7, Camry, Urus)...");
                context.Cars.RemoveRange(carsToRemove);
                context.SaveChanges();
                Console.WriteLine("[DbInitializer] Removed problematic cars.");
            }
            else
            {
                Console.WriteLine("[DbInitializer] No problematic cars found to remove.");
            }
        }
    }
}
