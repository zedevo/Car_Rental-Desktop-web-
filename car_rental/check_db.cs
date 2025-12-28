using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;

var optionsBuilder = new DbContextOptionsBuilder<CarRentalDbContext>();
optionsBuilder.UseSqlServer("Server=localhost,1433;Database=CarRentalDb;User Id=sa;Password=CarRentalAdmin123!;TrustServerCertificate=True;");

using var context = new CarRentalDbContext(optionsBuilder.Options);

try {
    Console.WriteLine($"Cars: {context.Cars.Count()}");
    Console.WriteLine($"Employees: {context.Employees.Count()}");
    Console.WriteLine($"Expenses: {context.Expenses.Count()}");
    Console.WriteLine($"Orders: {context.Orders.Count()}");
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
}
