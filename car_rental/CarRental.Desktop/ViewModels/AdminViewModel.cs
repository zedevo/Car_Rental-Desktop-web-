using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.ViewModels
{
    public partial class AdminViewModel : ViewModelBase
    {
        [ObservableProperty] private ObservableCollection<Employee> _employees = new();
        [ObservableProperty] private Employee? _selectedEmployee;
        [ObservableProperty] private ObservableCollection<Log> _logs = new();

        // New Employee Form
        [ObservableProperty] private string _newEmployeeName = string.Empty;
        [ObservableProperty] private string _newEmployeeLastName = string.Empty;
        [ObservableProperty] private string _newEmployeeEmail = string.Empty;
        [ObservableProperty] private string _newEmployeeUsername = string.Empty;
        [ObservableProperty] private string _newEmployeePassword = string.Empty;
        [ObservableProperty] private string _newEmployeeIdCard = string.Empty;
        [ObservableProperty] private int _newEmployeeAge = 25;
        [ObservableProperty] private string _newEmployeePhone = string.Empty;
        [ObservableProperty] private EmployeeRole _newEmployeeRole = EmployeeRole.Employee;

        public IEnumerable<EmployeeRole> RoleOptions => Enum.GetValues(typeof(EmployeeRole)).Cast<EmployeeRole>();
        public bool IsDeskAgent => NewEmployeeRole != EmployeeRole.Driver;
        partial void OnNewEmployeeRoleChanged(EmployeeRole value) => OnPropertyChanged(nameof(IsDeskAgent));

        public AdminViewModel()
        {
            LoadData();
        }

        [RelayCommand]
        private void LoadData()
        {
            using var context = CreateDbContext();
            
            Employees = new ObservableCollection<Employee>(
                context.Employees.OrderBy(e => e.Name).ToList());
                
            Logs = new ObservableCollection<Log>(
                context.Logs.Include(l => l.Employee)
                            .OrderByDescending(l => l.Timestamp)
                            .Take(100)
                            .ToList());
        }

        [RelayCommand]
        private void AddEmployee()
        {
            if (string.IsNullOrWhiteSpace(NewEmployeeName) || string.IsNullOrWhiteSpace(NewEmployeeLastName)) return;

            if (NewEmployeeRole != EmployeeRole.Driver)
            {
                if (string.IsNullOrWhiteSpace(NewEmployeeEmail) || string.IsNullOrWhiteSpace(NewEmployeeUsername)) return;
            }
                
            var emp = new Employee
            {
                Name = NewEmployeeName,
                LastName = NewEmployeeLastName,
                IdCard = NewEmployeeIdCard,
                Age = NewEmployeeAge,
                PhoneNumber = NewEmployeePhone,
                Email = IsDeskAgent ? NewEmployeeEmail : null,
                Username = IsDeskAgent ? NewEmployeeUsername : null,
                Password = IsDeskAgent ? NewEmployeePassword : null,
                Role = NewEmployeeRole
            };

            using var context = CreateDbContext();
            context.Employees.Add(emp);
            context.SaveChanges();
            
            LoadData();
            
            // Reset
            NewEmployeeName = string.Empty; 
            NewEmployeeLastName = string.Empty;
            NewEmployeeEmail = string.Empty; 
            NewEmployeeUsername = string.Empty;
            NewEmployeePassword = string.Empty;
            NewEmployeeIdCard = string.Empty;
            NewEmployeePhone = string.Empty;
        }

        [RelayCommand]
        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;
            using var context = CreateDbContext();
            context.Employees.Remove(SelectedEmployee);
            context.SaveChanges();
            LoadData();
        }
    }
}
