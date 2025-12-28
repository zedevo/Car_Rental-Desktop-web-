using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.QrCode;
using System.IO;

namespace CarRental.Desktop.Services
{
    public class PdfService
    {
        static PdfService()
        {
            // QuestPDF requires a license in v2022.12 and later. 
            // For community use, we set it to Community.
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GenerateContract(int orderId, string clientName, string carInfo, DateTime start, DateTime end, string outputPath)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Verdana));

                    page.Header().Text("Car Rental Contract")
                        .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(25).Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text($"Order ID: {orderId}").Bold();
                        column.Item().Text($"Client: {clientName}");
                        column.Item().Text($"Vehicle: {carInfo}");
                        column.Item().Text($"Rental Period: {start:d} to {end:d}");

                        column.Item().PaddingTop(20).Text("Legal Waiver").FontSize(16).SemiBold();
                        column.Item().Text("The lessee assumes all responsibility for the vehicle during the rental period. " +
                                           "The company is not liable for any traffic violations or accidents caused by negligence. " +
                                           "A security deposit is held until the safe return of the vehicle.");

                        // QR Code implementation
                        column.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem().Column(innerCol =>
                            {
                                innerCol.Item().Text("Scan to verify booking").Italic().FontSize(10);
                                innerCol.Item().Image(GenerateQRCode($"Order:{orderId}|Client:{clientName}|Valid:Yes"));
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            document.GeneratePdf(outputPath);
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
