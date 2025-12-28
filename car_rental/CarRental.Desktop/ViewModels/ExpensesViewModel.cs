using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarRental.Data.Models;
using CarRental.Data.Enums;
using System;
using System.Linq;

namespace CarRental.Desktop.ViewModels
{
    public partial class ExpensesViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Expense> _expenses = new();
        
        [ObservableProperty]
        private Expense? _selectedExpense;

        // Alerts
        [ObservableProperty]
        private ObservableCollection<string> _alerts = new();

        public ExpensesViewModel()
        {
            LoadExpenses();
            CheckAlerts();
        }

        [ObservableProperty]
        private decimal _totalRevenue;
        
        [ObservableProperty]
        private decimal _totalExpenses;
        
        [ObservableProperty]
        private decimal _netProfit;

        // Simple Chart Data (Text-based for now or basic progress)
        [ObservableProperty] private int _revenuePercentage = 50; // For a visual bar
        
        private void LoadExpenses()
        {
            try {
                using var context = CreateDbContext();
                var expenses = context.Expenses.OrderByDescending(e => e.Date).ToList();
                Expenses = new ObservableCollection<Expense>(expenses);
                
                TotalExpenses = expenses.Sum(e => e.Amount);
                
                // Calculate Revenue from Orders
                var revenue = context.Orders
                    .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Active)
                    .Sum(o => o.TotalAmount);
                TotalRevenue = revenue;
                
                NetProfit = TotalRevenue - TotalExpenses;
                
                // Avoid div by zero
                if (TotalRevenue + TotalExpenses > 0)
                    RevenuePercentage = (int)((TotalRevenue / (TotalRevenue + TotalExpenses)) * 100);

                Console.WriteLine($"[DEBUG] Loaded {expenses.Count} expenses and calculated {TotalRevenue:C} revenue.");
            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] LoadExpenses failed: {ex.Message}");
            }
        }

        private void CheckAlerts()
        {
            Alerts.Clear();
            using var context = CreateDbContext();
            
            // Logic: Find expenses (like insurance) that expire within 30 days
            var expiryThreshold = DateTime.Now.AddDays(30);
            var expiringItems = context.Expenses
                .Where(e => e.ExpiryDate != null && e.ExpiryDate <= expiryThreshold && e.ExpiryDate >= DateTime.Now)
                .ToList();

            foreach (var item in expiringItems)
            {
                var daysLeft = (item.ExpiryDate!.Value - DateTime.Now).Days;
                Alerts.Add($"ALERT: {item.Type} for {item.Description} expires in {daysLeft} days.");
            }
            
            var expiredItems = context.Expenses
                .Where(e => e.ExpiryDate != null && e.ExpiryDate < DateTime.Now)
                .ToList();
                
            foreach (var item in expiredItems)
            {
                Alerts.Add($"URGENT: {item.Type} for {item.Description} EXPIRED on {item.ExpiryDate!.Value.ToShortDateString()}. Renew immediately!");
            }
        }
        [ObservableProperty]
        private string _newDescription = string.Empty;
        
        [ObservableProperty]
        private decimal _newAmount;
        
        [ObservableProperty]
        private DateTimeOffset? _newExpiryDate;

        [ObservableProperty]
        private ExpenseType _newType = ExpenseType.Maintenance;

        public System.Collections.Generic.IEnumerable<ExpenseType> ExpenseTypes => Enum.GetValues(typeof(ExpenseType)).Cast<ExpenseType>();

        [RelayCommand]
        private void AddExpense()
        {
            if (string.IsNullOrWhiteSpace(NewDescription) || NewAmount <= 0)
                return;

            var expense = new Expense
            {
                Description = NewDescription,
                Amount = NewAmount,
                Type = NewType,
                Date = DateTime.Now,
                ExpiryDate = NewExpiryDate?.DateTime
            };

            using var context = CreateDbContext();
            context.Expenses.Add(expense);
            context.SaveChanges();
            
            Expenses.Insert(0, expense); // Add to top
            
            // Reset
            NewDescription = string.Empty;
            NewAmount = 0;
            NewExpiryDate = null;
            CheckAlerts(); // Refresh alerts
        }

        [RelayCommand]
        private void DeleteExpense()
        {
            if (SelectedExpense == null) return;
            
            using var context = CreateDbContext();
            var expense = context.Expenses.Find(SelectedExpense.Id);
            if (expense != null)
            {
                context.Expenses.Remove(expense);
                context.SaveChanges();
                Expenses.Remove(SelectedExpense);
                SelectedExpense = null;
                LoadExpenses(); // Refresh stats
                CheckAlerts();
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (SelectedExpense == null) return;
            
            using var context = CreateDbContext();
            context.Expenses.Update(SelectedExpense);
            context.SaveChanges();
            LoadExpenses();
            CheckAlerts();
        }
    }
}
