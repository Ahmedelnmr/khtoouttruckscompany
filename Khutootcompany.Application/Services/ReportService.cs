using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Interfaces;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Services
{

    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Set QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;
        }
        decimal totalIncome = 0;
        decimal totalExpense = 0;

        public async Task<byte[]> GeneratePAMReportAsync()
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksWithCurrentDriverAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("شركة الخطوط الدولية لنقل البضائع")
                                .FontSize(16).Bold().AlignRight();
                            column.Item().Text("كشف الشاحنات المسجلة في PAM")
                                .FontSize(14).AlignRight();
                            column.Item().Text($"التاريخ: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10).AlignRight();
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Table
                        column.Item().Table(table =>
                        {
                            // Define columns
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);  // ت
                                columns.RelativeColumn(1);    // التسجيل
                                columns.RelativeColumn(1);    // اللوحة
                                columns.RelativeColumn(1.5f); // الصنع
                                columns.RelativeColumn(0.8f); // السنة
                                columns.RelativeColumn(2);    // السائق
                                columns.RelativeColumn(1.5f); // المدني
                                columns.RelativeColumn(1);    // الجنسية
                                columns.RelativeColumn(1.5f); // الترخيص
                                columns.RelativeColumn(1);    // التأمين
                                columns.RelativeColumn(1);    // PAM
                                columns.RelativeColumn(1);    // اللون
                            });

                            // Header Row
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("ت").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("التسجيل").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("اللوحة").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("الصنع").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("السنة").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("السائق").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("المدني").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("الجنسية").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("الترخيص").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("التأمين").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("PAM").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(3)
                                    .Text("اللون").FontColor(Colors.White).Bold().AlignRight();
                            });

                            // Data Rows
                            int index = 1;
                            foreach (var truck in trucks.OrderBy(t => t.TruckId))
                            {
                                var currentDriver = truck.Assignments?.FirstOrDefault(a => a.IsCurrent);
                                var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                                // Color coding for expired insurance
                                if (truck.IsInsuranceExpired() || truck.IsInsuranceExpiringSoon())
                                {
                                    bgColor = Colors.Red.Lighten3;
                                }

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(index.ToString()).AlignCenter();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.PAMRegistrationDate?.ToString("dd/MM/yy") ?? "").AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.PlateNumber).Bold().AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.Model).AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.Year?.ToString() ?? "").AlignCenter();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(currentDriver?.Employee?.FullName ?? "").AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(currentDriver?.Employee?.CivilId ?? "").AlignCenter();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(currentDriver?.Employee?.Nationality?.ToString().Replace("_", " ") ?? "").AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.LicenseType.ToString().Replace("_", " ")).AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.InsuranceExpiryDate?.ToString("dd/MM/yy") ?? "").AlignCenter();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.PAMStatus?.ToString().Replace("_", " ") ?? "").AlignRight();

                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3).Text(truck.Color ?? "").AlignRight();

                                index++;
                            }
                        });

                        // Summary
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Text($"إجمالي الشاحنات: {trucks.Count()}")
                                .FontSize(11).Bold().AlignRight();
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("صفحة ");
                        text.CurrentPageNumber();
                        text.Span(" من ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateExpiredResidenciesReportAsync()
        {
            var employees = await _unitOfWork.Employees.GetEmployeesWithExpiredResidencyAsync();
            var expiringSoon = await _unitOfWork.Employees.GetEmployeesWithExpiringSoonResidencyAsync(30);
            var allExpired = employees.Concat(expiringSoon).Distinct();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("كشف الإقامات المنتهية")
                            .FontSize(18).Bold().AlignCenter();
                        column.Item().Text($"التاريخ: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(11).AlignCenter();
                        column.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("ت").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("الاسم").FontColor(Colors.White).Bold().AlignRight();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("الرقم المدني").FontColor(Colors.White).Bold().AlignRight();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("الوظيفة").FontColor(Colors.White).Bold().AlignRight();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("انتهاء الإقامة").FontColor(Colors.White).Bold().AlignCenter();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("الحالة").FontColor(Colors.White).Bold().AlignCenter();
                        });

                        int index = 1;
                        foreach (var emp in allExpired.OrderBy(e => e.ResidencyExpiryDate))
                        {
                            var isExpired = emp.IsResidencyExpired();
                            var bgColor = isExpired ? Colors.Red.Lighten3 : Colors.Orange.Lighten3;
                            var status = isExpired ? "منتهي" : "ينتهي قريباً";

                            table.Cell().Background(bgColor).Padding(5).Text(index.ToString());
                            table.Cell().Background(bgColor).Padding(5).Text(emp.FullName).AlignRight();
                            table.Cell().Background(bgColor).Padding(5).Text(emp.CivilId).AlignRight();
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(emp.JobTitle.ToString().Replace("_", " ")).AlignRight();
                            table.Cell().Background(bgColor).Padding(5)
                                .Text(emp.ResidencyExpiryDate?.ToString("dd/MM/yyyy") ?? "").AlignCenter();
                            table.Cell().Background(bgColor).Padding(5).Text(status).Bold().AlignCenter();

                            index++;
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateOverdueInstallmentsReportAsync()
        {
            var installments = await _unitOfWork.Installments.GetOverdueInstallmentsAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("كشف الأقساط المتأخرة")
                            .FontSize(18).Bold().AlignCenter();
                        column.Item().Text($"التاريخ: {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(11).AlignCenter();
                        column.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Red.Darken2).Padding(5)
                                .Text("ت").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Red.Darken2).Padding(5)
                                .Text("اللوحة").FontColor(Colors.White).Bold().AlignRight();
                            header.Cell().Background(Colors.Red.Darken2).Padding(5)
                                .Text("السائق").FontColor(Colors.White).Bold().AlignRight();
                            header.Cell().Background(Colors.Red.Darken2).Padding(5)
                                .Text("القسط الشهري").FontColor(Colors.White).Bold().AlignCenter();
                            header.Cell().Background(Colors.Red.Darken2).Padding(5)
                                .Text("المدفوع").FontColor(Colors.White).Bold().AlignCenter();
                            header.Cell().Background(Colors.Red.Darken2).Padding(5)
                                .Text("المتبقي").FontColor(Colors.White).Bold().AlignCenter();
                        });

                        int index = 1;
                        foreach (var inst in installments)
                        {
                            table.Cell().Background(Colors.Red.Lighten4).Padding(5).Text(index.ToString());
                            table.Cell().Background(Colors.Red.Lighten4).Padding(5)
                                .Text(inst.Truck?.PlateNumber ?? "").Bold().AlignRight();
                            table.Cell().Background(Colors.Red.Lighten4).Padding(5)
                                .Text(inst.Employee?.FullName ?? "").AlignRight();
                            table.Cell().Background(Colors.Red.Lighten4).Padding(5)
                                .Text($"{inst.MonthlyQest:N3} د.ك").AlignCenter();
                            table.Cell().Background(Colors.Red.Lighten4).Padding(5)
                                .Text($"{inst.PaidAmount:N3} د.ك").AlignCenter();
                            table.Cell().Background(Colors.Red.Lighten4).Padding(5)
                                .Text($"{inst.RemainingAmount:N3} د.ك").Bold().AlignCenter();

                            index++;
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateExpiredWakalatReportAsync()
        {
            var wakalat = await _unitOfWork.Wakalat.GetExpiredWakalatAsync();
            var expiringSoon = await _unitOfWork.Wakalat.GetExpiringSoonWakalatAsync(30);
            var allExpired = wakalat.Concat(expiringSoon).Distinct();

            // Similar implementation to residencies report
            return await Task.FromResult(new byte[0]); // Placeholder
        }

        public async Task<byte[]> GenerateExpiredInsuranceReportAsync()
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksWithExpiredInsuranceAsync();
            var expiringSoon = await _unitOfWork.Trucks.GetTrucksWithExpiringSoonInsuranceAsync(30);
            var allExpired = trucks.Concat(expiringSoon).Distinct();

            // Similar implementation
            return await Task.FromResult(new byte[0]); // Placeholder
        }

        public async Task<byte[]> GenerateMonthlyCashReportAsync(int year, int month)
        {
            var transactions = await _unitOfWork.CashTransactions.GetMonthlyTransactionsAsync(year, month);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Column(column =>
                    {
                        column.Item().Text($"تقرير الصندوق - {month}/{year}")
                            .FontSize(16).Bold().AlignCenter();
                        column.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                    .Text("التاريخ").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                    .Text("النوع").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                    .Text("المبلغ").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                    .Text("الموظف").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                    .Text("السند").FontColor(Colors.White).Bold().AlignCenter();
                                header.Cell().Background(Colors.Green.Darken2).Padding(4)
                                    .Text("الوصف").FontColor(Colors.White).Bold().AlignRight();
                            });

                    

                            foreach (var trans in transactions.OrderBy(t => t.TransactionDate))
                            {
                                var bgColor = trans.Amount > 0 ? Colors.Green.Lighten4 : Colors.Red.Lighten4;

                                if (trans.Amount > 0) totalIncome += trans.Amount;
                                else totalExpense += Math.Abs(trans.Amount);

                                table.Cell().Background(bgColor).Padding(4)
                                    .Text(trans.TransactionDate.ToString("dd/MM")).AlignCenter();
                                table.Cell().Background(bgColor).Padding(4)
                                    .Text(trans.Type.ToString().Replace("_", " ")).AlignRight();
                                table.Cell().Background(bgColor).Padding(4)
                                    .Text($"{trans.Amount:N3}").Bold().AlignCenter();
                                table.Cell().Background(bgColor).Padding(4)
                                    .Text(trans.Employee?.FullName ?? "").AlignRight();
                                table.Cell().Background(bgColor).Padding(4)
                                    .Text(trans.SondNumber ?? "").AlignCenter();
                                table.Cell().Background(bgColor).Padding(4)
                                    .Text(trans.Description ?? "").AlignRight();
                            }
                        });

                        // Summary
                        col.Item().PaddingTop(15).Column(summary =>
                        {
                            summary.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"إجمالي القبض: {totalIncome:N3} د.ك")
                                    .FontSize(12).Bold().FontColor(Colors.Green.Darken2);
                            });
                            summary.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"إجمالي الصرف: {totalExpense:N3} د.ك")
                                    .FontSize(12).Bold().FontColor(Colors.Red.Darken2);
                            });
                            summary.Item().PaddingTop(5).LineHorizontal(1);
                            summary.Item().Row(row =>
                            {
                                var net = totalIncome - totalExpense;
                                var color = net >= 0 ? Colors.Green.Darken3 : Colors.Red.Darken3;
                                row.RelativeItem().Text($"الصافي: {net:N3} د.ك")
                                    .FontSize(14).Bold().FontColor(color);
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
