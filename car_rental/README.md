# Aurum Veloce - Car Rental Management System

Aurum Veloce is a comprehensive car rental solution designed for both customers and administrators. It features a modern, responsive **Web Application** for clients and a robust **Desktop Application** for internal management.

## 🚀 Experience the Future of Car Rentals
![Aurum Veloce Home](/home/spook/.gemini/antigravity/brain/40df1b12-f38c-436b-9d24-be0ee2ea2df9/uploaded_image_0_1766822992532.png)

## ✨ Core Features

### 🌐 Web Application (Customer Side)
- **Luxury Fleet Browsing**: Real-time availability and dynamic pricing.
- **Fast Booking System**: Book a vehicle in under 60 seconds.
- **User Profile Management**: Secure personal data and booking history (GDPR compliant).
- **Responsive Design**: Premium look and feel optimized for all devices.

### 🖥️ Desktop Application (Admin Side)
- **Operations Dashboard**: Real-time stats, fleet status, and operations calendar.
- **Fleet Management**: Easily add, edit, and track vehicles.
- **Financial Operations**: Track expenses, maintenance costs, and generate reports.
- **HR & Staff Management**: Manage employees, drivers, and access levels.
- **Audit Logs**: Full traceability of all system actions.

## 🛠️ Tech Stack

- **Backend**: C# / .NET 9
- **Web**: ASP.NET Core MVC
- **Desktop**: Avalonia UI (Cross-platform XAML)
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core (Code First)
- **Diagrams**: UML (TikZ/Mermaid)

## 📸 Screenshots

| Customer Fleet View | Admin Dashboard |
| :---: | :---: |
| ![Fleet](/home/spook/.gemini/antigravity/brain/40df1b12-f38c-436b-9d24-be0ee2ea2df9/uploaded_image_4_1766822992532.png) | ![Dashboard](/home/spook/.gemini/antigravity/brain/40df1b12-f38c-436b-9d24-be0ee2ea2df9/uploaded_image_0_1766823290124.png) |

| Financial Management | Audit Logs |
| :---: | :---: |
| ![Financials](/home/spook/.gemini/antigravity/brain/40df1b12-f38c-436b-9d24-be0ee2ea2df9/uploaded_image_2_1766823290124.png) | ![Logs](/home/spook/.gemini/antigravity/brain/40df1b12-f38c-436b-9d24-be0ee2ea2df9/uploaded_image_4_1766823290124.png) |

## ⚙️ Installation & Setup

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB or Server)

### 1. Database Configuration
Update the connection string in `CarRental.Web/appsettings.json` and `CarRental.Desktop/.../appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CarRentalDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 2. Run the Web Application
```bash
cd CarRental.Web
dotnet run
```

### 3. Run the Desktop Application
```bash
cd CarRental.Desktop
dotnet run
```

## 👥 Authors
- **Ziad Zarhoun**
- **Yassine Salihi**

---
*Developed as a Final Degree Project (PFE) with focus on modern architecture and premium UX.*
