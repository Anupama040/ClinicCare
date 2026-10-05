using ClinicManagement.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicManagement.Services;

public class PrescriptionPdfService
{
    public byte[] Generate(Prescription prescription)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.DefaultTextStyle(
                    style => style.FontSize(11));

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignCenter()
                            .Text("CLINIC MANAGEMENT")
                            .Bold()
                            .FontSize(22)
                            .FontColor(Colors.Blue.Medium);

                        column.Item()
                            .AlignCenter()
                            .Text("Medical Prescription")
                            .FontSize(15);

                        column.Item()
                            .PaddingTop(10)
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Medium);
                    });

                page.Content()
                    .PaddingTop(20)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text(text =>
                            {
                                text.Span("Prescription ID: ")
                                    .Bold();

                                text.Span(
                                    prescription.Id.ToString());
                            });

                        column.Item()
                            .Text(text =>
                            {
                                text.Span("Date: ")
                                    .Bold();

                                text.Span(
                                    prescription.CreatedAt
                                        .ToLocalTime()
                                        .ToString("dd-MM-yyyy"));
                            });

                        column.Item()
                            .PaddingTop(10)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Column(patient =>
                            {
                                patient.Spacing(5);

                                patient.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Patient: ")
                                            .Bold();

                                        text.Span(
                                            $"{prescription.Patient.FirstName} "
                                            + $"{prescription.Patient.LastName}");
                                    });

                                patient.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Phone: ")
                                            .Bold();

                                        text.Span(
                                            prescription.Patient.Phone);
                                    });

                                patient.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Doctor: ")
                                            .Bold();

                                        text.Span(
                                            $"Dr. {prescription.Doctor.FirstName} "
                                            + $"{prescription.Doctor.LastName}");
                                    });

                                patient.Item()
                                    .Text(text =>
                                    {
                                        text.Span("Specialization: ")
                                            .Bold();

                                        text.Span(
                                            prescription.Doctor.Specialization);
                                    });
                            });

                        column.Item()
                            .PaddingTop(15)
                            .Text(text =>
                            {
                                text.Span("Diagnosis: ")
                                    .Bold();

                                text.Span(
                                    prescription.Diagnosis
                                    ?? "Not provided");
                            });

                        column.Item()
                            .PaddingTop(15)
                            .Text("Medicines")
                            .Bold()
                            .FontSize(14);

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell()
                                        .Background(Colors.Blue.Medium)
                                        .Padding(5)
                                        .Text("Medicine")
                                        .FontColor(Colors.White)
                                        .Bold();

                                    header.Cell()
                                        .Background(Colors.Blue.Medium)
                                        .Padding(5)
                                        .Text("Dosage")
                                        .FontColor(Colors.White)
                                        .Bold();

                                    header.Cell()
                                        .Background(Colors.Blue.Medium)
                                        .Padding(5)
                                        .Text("Frequency")
                                        .FontColor(Colors.White)
                                        .Bold();

                                    header.Cell()
                                        .Background(Colors.Blue.Medium)
                                        .Padding(5)
                                        .Text("Duration")
                                        .FontColor(Colors.White)
                                        .Bold();

                                    header.Cell()
                                        .Background(Colors.Blue.Medium)
                                        .Padding(5)
                                        .Text("Instructions")
                                        .FontColor(Colors.White)
                                        .Bold();
                                });

                                foreach (var item in prescription.Items)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5)
                                        .Text(item.MedicineName);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5)
                                        .Text(item.Dosage);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5)
                                        .Text(item.Frequency);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5)
                                        .Text(item.Duration);

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5)
                                        .Text(item.Instructions ?? "None");
                                }
                            });

                        column.Item()
                            .PaddingTop(35)
                            .Text(
                                "Doctor signature: "
                                + "____________________________");

                        column.Item()
                            .PaddingTop(20)
                            .Text(
                                "This prescription was generated "
                                + "by Clinic Management.");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Clinic Management | Page ");
                        text.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf();
    }
}