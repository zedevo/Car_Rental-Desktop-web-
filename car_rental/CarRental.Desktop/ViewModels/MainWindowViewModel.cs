using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using CarRental.Desktop.Services;
using System.IO;

namespace CarRental.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentPage;
        
        [ObservableProperty]
        private bool _isLoggedIn = false;
        
        [ObservableProperty]
        private bool _isAdmin = false;
        
        [ObservableProperty]
        private string _currentUserDisplay = string.Empty;

        public MainWindowViewModel()
        {
            ShowLogin();
        }

        private void ShowLogin()
        {
            var loginVm = new LoginViewModel();
            loginVm.OnLoginSuccess = (user) =>
            {
                Console.WriteLine($"[DEBUG] Login success for: {user.Email} (Role: {user.Role})");
                IsLoggedIn = true;
                CurrentUserDisplay = $"{user.Name} ({user.Role})";
                IsAdmin = user.Role == EmployeeRole.Admin;
                
                if (IsAdmin)
                {
                    Console.WriteLine("[DEBUG] Navigating to Dashboard as Admin.");
                    GoToDashboard();
                }
                else
                {
                    Console.WriteLine("[DEBUG] Navigating to Operations Hub as Desk Worker.");
                    GoToOperations();
                }
            };
            CurrentPage = loginVm;
        }

        [RelayCommand]
        private void Logout()
        {
            Console.WriteLine("[DEBUG] Logging out.");
            
            if (ViewModelBase.CurrentUser != null)
            {
                using var context = CreateDbContext();
                context.Logs.Add(new Log { 
                    EmployeeId = ViewModelBase.CurrentUser.Id, 
                    Action = "Logged out of the system",
                    Timestamp = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            IsLoggedIn = false;
            IsAdmin = false;
            ViewModelBase.CurrentUser = null;
            ShowLogin();
        }

        partial void OnCurrentPageChanged(ViewModelBase value)
        {
            Console.WriteLine($"[DEBUG] Navigation: CurrentPage changed to {value?.GetType().Name ?? "null"}");
        }

        [RelayCommand]
        private void GoToDashboard() 
        {
            Console.WriteLine("[DEBUG] Menu: Dashboard clicked.");
            CurrentPage = new DashboardViewModel();
        }

        [RelayCommand]
        private void GoToCars() 
        {
            Console.WriteLine("[DEBUG] Menu: Fleet clicked.");
            CurrentPage = new CarsViewModel();
        }
        
        [RelayCommand]
        private void GoToOperations() 
        {
            Console.WriteLine("[DEBUG] Menu: Operations clicked.");
            CurrentPage = new OperationsViewModel();
        }
        
        [RelayCommand]
        private void GoToExpenses() 
        {
            Console.WriteLine("[DEBUG] Menu: Expenses clicked.");
            CurrentPage = new ExpensesViewModel();
        }

        [RelayCommand]
        private void GoToAdmin()
        {
            Console.WriteLine("[DEBUG] Menu: Admin Console clicked.");
            CurrentPage = new AdminViewModel();
        }
    }
}
