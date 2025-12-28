using Microsoft.AspNetCore.Mvc;
using CarRental.Data;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Web.Controllers
{
    public class FleetController : Controller
    {
        private readonly CarRentalDbContext _context;

        public FleetController(CarRentalDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!_context.Cars.Any() || _context.Cars.Count() < 10)
            {
                await SeedCars();
            }

            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        private async Task SeedCars()
        {
            var makeModels = new[] 
            { 
                ("Porsche", "911 Carrera", "Luxury", 250m, "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&q=80&w=800"),
                ("Mercedes", "S-Class", "Luxury", 200m, "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&q=80&w=800"),
                ("BMW", "M5 Competition", "Sport", 180m, "https://images.unsplash.com/photo-1555215695-3004980adade?auto=format&fit=crop&q=80&w=800"),
                ("Audi", "RS7", "Sport", 190m, "https://images.unsplash.com/photo-1603584173870-7b299f589389?auto=format&fit=crop&q=80&w=800"),
                ("Volvo", "9700 Bus", "Bus", 500m, "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?auto=format&fit=crop&q=80&w=800"),
                ("Tesla", "Model S Plaid", "Electric", 160m, "https://images.unsplash.com/photo-1536700503339-1e4b06520771?auto=format&fit=crop&q=80&w=800"),
                ("Range Rover", "Autobiography", "SUV", 220m, "https://images.unsplash.com/photo-1606132731872-91f165249511?auto=format&fit=crop&q=80&w=800"),
                ("Lamborghini", "Urus", "Super SUV", 350m, "https://images.unsplash.com/photo-1616892019460-60b64267711b?auto=format&fit=crop&q=80&w=800"),
                ("Toyota", "Camry Hybrid", "Economy", 50m, "https://images.unsplash.com/photo-1621007947382-bb3c3968e3bb?auto=format&fit=crop&q=80&w=800"),
                ("Ford", "Mustang GT", "Muscle", 120m, "https://images.unsplash.com/photo-1584345604476-8ec5e12e42dd?auto=format&fit=crop&q=80&w=800")
            };

            foreach (var (make, model, type, price, img) in makeModels)
            {
                if (!await _context.Cars.AnyAsync(c => c.Make == make && c.Model == model))
                {
                    _context.Cars.Add(new Data.Models.Car
                    {
                        Make = make,
                        Model = model,
                        Year = 2024,
                        PlateNumber = $"{make[0..2].ToUpper()}-{new Random().Next(1000, 9999)}",
                        Category = type,
                        DailyRate = price,
                        ImageUrl = img,
                        IsAvailable = true
                    });
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
