using Avalonia;
using System;
using Microsoft.EntityFrameworkCore;
using CarRental.Desktop;

using CarRental.Desktop;

namespace CarRental.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        EnsureAdminUser();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
            
    private static void EnsureAdminUser()
    {
        try
        {
            Console.WriteLine("[INFO] Starting database initialization...");
            var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<CarRental.Data.CarRentalDbContext>();
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=CarRentalDb;User Id=sa;Password=CarRentalAdmin123!;TrustServerCertificate=True;");

            using var context = new CarRental.Data.CarRentalDbContext(optionsBuilder.Options);
            context.Database.Migrate();
            
            var adminExists = System.Linq.Enumerable.Any(context.Employees, e => e.Role == CarRental.Data.Enums.EmployeeRole.Admin);
            
            if (!adminExists)
            {
                var admin = new CarRental.Data.Models.Employee
                {
                    Name = "System Admin",
                    Email = "admin@aurum.com",
                    Password = "AdminPass123!",
                    Role = CarRental.Data.Enums.EmployeeRole.Admin,
                    DriverStatus = CarRental.Data.Enums.DriverStatus.Available,
                    IsClockedIn = true
                };
                context.Employees.Add(admin);
                context.SaveChanges();
                Console.WriteLine("[INFO] Admin user seeded: admin@aurum.com");
            }
            else
            {
                Console.WriteLine("[INFO] Admin user already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Seeding failed: {ex.Message}");
        }
    }
}
