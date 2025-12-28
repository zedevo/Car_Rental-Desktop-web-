using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using System.Linq;
using System;

namespace CarRental.Desktop.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public System.Action<Employee>? OnLoginSuccess { get; set; }

        [RelayCommand]
        private void Login()
        {
            Console.WriteLine($"[DEBUG] Login attempt for: {Email}");
            ErrorMessage = string.Empty;
            try {
                using var context = CreateDbContext();
                var user = context.Employees.FirstOrDefault(e => e.Email == Email && e.Password == Password);
                
                if (user != null)
                {
                    Console.WriteLine("[DEBUG] Credentials valid. Proceeding...");
                    
                    // Audit Log
                    context.Logs.Add(new Log { 
                        EmployeeId = user.Id, 
                        Action = "Logged in to the system",
                        Timestamp = DateTime.UtcNow
                    });
                    context.SaveChanges();

                    ViewModelBase.CurrentUser = user;
                    OnLoginSuccess?.Invoke(user);
                }
                else
                {
                    Console.WriteLine("[DEBUG] Credentials invalid.");
                    ErrorMessage = "Invalid credentials.";
                }
            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] Login failed: {ex.Message}");
                ErrorMessage = "Database connection error.";
            }
        }
    }
}
