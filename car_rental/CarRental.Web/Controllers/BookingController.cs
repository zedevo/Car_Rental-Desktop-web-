using Microsoft.AspNetCore.Mvc;
using CarRental.Web.Models;
using CarRental.Data;
using CarRental.Data.Models;
using CarRental.Data.Enums; // Added for Enums like OrderStatus
using Microsoft.AspNetCore.SignalR;
using CarRental.Web.Hubs;
using Microsoft.AspNetCore.Identity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.QrCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CarRental.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly CarRentalDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(CarRentalDbContext context, IHubContext<NotificationHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new BookingViewModel();
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.ClientName = $"{user.FirstName} {user.LastName}".Trim();
                    model.ContactInfo = user.Email ?? "";
                    // Could fetch existing client profile here
                    var existingClient = _context.Clients.FirstOrDefault(c => c.ContactInfo == user.Email);
                    if (existingClient != null)
                    {
                        model.ClientName = existingClient.Name;
                        model.ClientType = existingClient.Type;
                        model.RegistrationNumber = existingClient.RegistrationNumber;
                        model.NationalId = existingClient.NationalId;
                        model.DriverLicenseNumber = existingClient.DriverLicenseNumber;
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Check if client exists by email (ContactInfo) to avoid duplicates
            var client = _context.Clients.FirstOrDefault(c => c.ContactInfo == model.ContactInfo);
            if (client == null)
            {
                client = new Client
                {
                    Name = model.ClientName,
                    ContactInfo = model.ContactInfo,
                    Type = model.ClientType,
                    RegistrationNumber = model.RegistrationNumber,
                    PassengerCount = model.PassengerCount,
                    NationalId = model.NationalId,
                    DriverLicenseNumber = model.DriverLicenseNumber,
                    CreditCardNumber = model.CreditCardNumber
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update existing client info if needed
                client.Name = model.ClientName;
                client.Type = model.ClientType;
                client.RegistrationNumber = model.RegistrationNumber;
                client.PassengerCount = model.PassengerCount;
                client.NationalId = model.NationalId;
                client.DriverLicenseNumber = model.DriverLicenseNumber;
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }

            // Create Order
            var order = new Order
            {
                ClientId = client.Id,
                Status = OrderStatus.Pending,
                StartDate = model.PickupDate,
                EndDate = model.EndDate,
                PickupLocation = model.PickupLocation,
                Destination = model.Destination,
                RequestedCarType = model.SelectedCarType,
                SecurityDeposit = model.SecurityDeposit ?? 0, // Store deposit
                NumberOfPassengers = model.PassengerCount ?? 0,
                TotalAmount = 0 
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Notify Admin
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", $"New Order #{order.Id} from {client.Name}!", order.Id);

            return RedirectToAction("Success", new { id = order.Id });
        }
        
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        public async Task<IActionResult> DownloadWaiver(int id)
        {
            var order = await _context.Orders.Include(o => o.Client).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("AURUM VELOCE - RENTAL AGREEMENT WAIVER")
                        .SemiBold().FontSize(20).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);
                            x.Item().Text($"Date: {DateTime.Now:d}");
                            x.Item().Text($"Order ID: {order.Id}");
                            x.Item().Text($"Client: {order.Client.Name}");
                            x.Item().Text($"Type: {order.Client.Type}");
                            
                            if (order.Client.Type == ClientType.Organization)
                            {
                                x.Item().Text($"Registration No: {order.Client.RegistrationNumber}");
                                x.Item().Text($"Passengers: {order.NumberOfPassengers}");
                                x.Item().Text($"Security Deposit: {order.SecurityDeposit:C}");
                            }
                            else
                            {
                                x.Item().Text($"Driver License: {order.Client.DriverLicenseNumber}");
                                x.Item().Text($"National ID: {order.Client.NationalId}");
                            }

                            x.Item().LineHorizontal(1);
                            x.Item().Text("LEGAL WAIVER").Bold().FontSize(14);
                            x.Item().Text("By signing this document, the client agrees to the terms and conditions...");
                            
                            // Generate QR Code
                            // SkiaSharp.QrCode usage if static
                            var qr = QRCodeGenerator.CreateQrCode($"https://aurumveloce.com/verify/{order.Id}", ECCLevel.Q);
                            var info = new SKImageInfo(200, 200);
                            using var surface = SKSurface.Create(info);
                            var canvas = surface.Canvas;
                            canvas.Clear(SKColors.White);
                            canvas.Render(qr, info.Width, info.Height);
                            
                            using var image = surface.Snapshot();
                            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                            var qrBytes = data.ToArray();

                            x.Item().AlignRight().Image(qrBytes).FitArea();
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", $"Waiver_Order_{id}.pdf");
        }
    }
}

