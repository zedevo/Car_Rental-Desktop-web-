using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Desktop.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public static CarRental.Data.Models.Employee? CurrentUser { get; set; }

    protected CarRental.Data.CarRentalDbContext CreateDbContext()
    {
        var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<CarRental.Data.CarRentalDbContext>();
        optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=CarRentalDb;User Id=sa;Password=CarRentalAdmin123!;TrustServerCertificate=True;");
        return new CarRental.Data.CarRentalDbContext(optionsBuilder.Options);
    }
}
