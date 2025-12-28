using Microsoft.EntityFrameworkCore;
using CarRental.Data;

using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Email Service
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, CarRental.Web.Services.EmailSender>();

builder.Services.AddDbContext<CarRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<CarRental.Data.Models.ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CarRentalDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    // Fix for 400 Bad Request on HTTP (Development)
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

builder.Services.AddAntiforgery(options =>
{
    // Fix for 400 Bad Request on HTTP (Development) for Anti-Forgery Token
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? "dummy_ms_id";
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? "dummy_ms_secret";
    });

// Twilio Configuration (Placeholder)
var twilioAccountSid = builder.Configuration["Twilio:AccountSid"];
var twilioAuthToken = builder.Configuration["Twilio:AuthToken"];
// builder.Services.AddSingleton(new TwilioService(twilioAccountSid, twilioAuthToken)); // Implement Service later

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    // Seed Database in Development
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<CarRentalDbContext>();
        DbInitializer.Initialize(context);
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<CarRental.Web.Hubs.NotificationHub>("/notificationHub");
app.MapRazorPages();

app.Run();
