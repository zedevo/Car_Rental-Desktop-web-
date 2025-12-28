using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Data.Models;

namespace CarRental.Web.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly CarRentalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientController(CarRentalDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyBookings()
        {
             // For this demo, since we don't have a direct link between IdentityUser and Client table yet,
             // we will assume the Client.ContactInfo (Email) matches IdentityUser.Email.
             
             var user = await _userManager.GetUserAsync(User);
             if (user == null || user.Email == null) return View(new List<Order>());

             var client = await _context.Clients.FirstOrDefaultAsync(c => c.ContactInfo == user.Email);
             
             if (client == null)
             {
                 ViewBag.Message = "No bookings found matching your email.";
                 return View(new List<Order>());
             }

             var orders = await _context.Orders
                 .Include(o => o.Car)
                 .Where(o => o.ClientId == client.Id)
                 .OrderByDescending(o => o.StartDate)
                 .ToListAsync();

             return View(orders);
        }
    }
}
