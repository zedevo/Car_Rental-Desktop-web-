using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.QrCode;
using System.IO;

namespace CarRental.Web.Services
{
    public class PdfService
    {
        static PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateContractBytes(int orderId, string clientName, string carInfo, DateTime start, DateTime end, decimal totalAmount)
        {
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
            byte[] logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : new byte[0];

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Element(header => 
                    {
                        header.Row(row => 
                        {
                            // Logo and Brand
                            row.RelativeItem().Column(col => 
                            {
                                if (logoBytes.Length > 0)
                                    col.Item().Height(40).Image(logoBytes).FitArea();
                                
                                col.Item().Text("AURUM VELOCE").FontSize(20).Bold().FontColor("#D4AF37"); // Gold
                                col.Item().Text("Premium Car Rental Services").FontSize(9).FontColor(Colors.Grey.Medium);
                            });

                            // Company Info
                            row.RelativeItem().AlignRight().Column(col => 
                            {
                                col.Item().Text("INVOICE / AGREEMENT").FontSize(16).Bold().FontColor(Colors.Black);
                                col.Item().Text($"Ref: ORD-{orderId:D6}").FontSize(10).SemiBold();
                                col.Item().Text($"Date: {DateTime.Now:d}").FontSize(10);
                                col.Item().Text("Marrakech, Morocco").FontSize(9).Italic();
                            });
                        });
                        header.PaddingBottom(10);
                        header.LineHorizontal(1).LineColor("#D4AF37");
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        // Customer & Rental Details Section
                        column.Item().Row(row => 
                        {
                            row.RelativeItem().Column(col => 
                            {
                                col.Item().Text("Customer Details").FontSize(12).Bold().FontColor("#D4AF37");
                                col.Item().Text(clientName).FontSize(14).SemiBold();
                                col.Item().Text("Valued Client").FontSize(10).FontColor(Colors.Grey.Darken2);
                            });

                            row.RelativeItem().Column(col => 
                            {
                                col.Item().Text("Rental Period").FontSize(12).Bold().FontColor("#D4AF37");
                                col.Item().Text($"{start:MMM dd, yyyy} - {end:MMM dd, yyyy}").FontSize(11);
                                col.Item().Text($"Duration: {(end - start).Days} Days").FontSize(10);
                            });
                        });

                        column.Item().PaddingTop(20).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                        // Vehicle Details Table
                        column.Item().PaddingTop(10).Table(table => 
                        {
                            table.ColumnsDefinition(columns => 
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header => 
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Vehicle Description");
                                header.Cell().Element(CellStyle).AlignRight().Text("Rate/Day");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");
                                
                                static IContainer CellStyle(IContainer container) 
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                                                    .PaddingVertical(5).PaddingHorizontal(10)
                                                    .Background("#05070a"); // Dark Charcoal Background
                                }
                            });

                            table.Cell().Element(CellStyle).Text("1");
                            table.Cell().Element(CellStyle).Text(carInfo).SemiBold();
                            // Assuming totalAmount is passed, calculating rate roughly or just showing Total
                            table.Cell().Element(CellStyle).AlignRight().Text("-"); 
                            table.Cell().Element(CellStyle).AlignRight().Text($"{totalAmount:C}");

                            static IContainer CellStyle(IContainer container) 
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                                                .PaddingVertical(10).PaddingHorizontal(10);
                            }
                        });
                        
                        // Total
                        column.Item().PaddingTop(10).AlignRight().Row(row => 
                        {
                            row.ConstantColumn(150).Text("Grand Total:").FontSize(14).Bold();
                            row.ConstantColumn(100).AlignRight().Text($"{totalAmount:C}").FontSize(14).Bold().FontColor("#D4AF37");
                        });

                        // Terms
                        column.Item().PaddingTop(30).Text("Terms & Conditions").FontSize(12).Bold().FontColor("#D4AF37");
                        column.Item().PaddingTop(5).Text("1. The lessee assumes all responsibility for the vehicle during the rental period.").FontSize(9);
                        column.Item().Text("2. The company is not liable for any traffic violations or accidents caused by negligence.").FontSize(9);
                        column.Item().Text("3. A security deposit is held until the safe return of the vehicle.").FontSize(9);
                        column.Item().Text("4. Late returns will be charged an additional fee equivalent to one day's rental.").FontSize(9);

                        // QR Code
                        column.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem().Column(innerCol =>
                            {
                                innerCol.Item().Text("Scan to verify booking validity").Italic().FontSize(9).FontColor(Colors.Grey.Medium);
                                innerCol.Item().Width(80).Image(GenerateQRCode($"Order:{orderId}|Client:{clientName}|Amount:{totalAmount:C}"));
                            });
                            
                            row.RelativeItem().AlignRight().AlignBottom().Column(col => 
                            {
                                col.Item().LineHorizontal(1).LineColor(Colors.Black);
                                col.Item().PaddingTop(5).Text("Authorized Signature").FontSize(10);
                            });
                        });
                    });

                    page.Footer().Column(col => 
                    {
                        col.Item().LineHorizontal(1).LineColor("#D4AF37");
                        col.Item().PaddingTop(10).Row(row => 
                        {
                            row.RelativeItem().Text("Aurum Veloce Luxury Rentals").FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight().Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private byte[] GenerateQRCode(string text)
        {
            var qr = QRCodeGenerator.CreateQrCode(text, ECCLevel.Q);
            var info = new SKImageInfo(200, 200);
            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);
                canvas.Render(qr, 200, 200, SKColors.Black, SKColors.White);
                
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return data.ToArray();
                }
            }
        }
    }
}
