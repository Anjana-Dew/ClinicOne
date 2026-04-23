using ClinicOne.Models.ViewModels.Pharmacist;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
﻿using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ClinicOne.Models.ViewModels.Pharmacist;

namespace ClinicOne.Services
{
    public class ExternalPrescriptionDocument : IDocument
    {
        private readonly ExternalPrescriptionPdfModel _model;

        public ExternalPrescriptionDocument(ExternalPrescriptionPdfModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header().Text("ClinicOne Pharmacy")
                    .FontSize(20).Bold().AlignCenter();

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Patient: {_model.PatientName}");
                    col.Item().Text($"NIC: {_model.NIC}");
                    col.Item().Text($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3); // Medicine
                            c.RelativeColumn(2); // Dosage
                            c.RelativeColumn(2); // Duration
                            c.RelativeColumn(1); // Times
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Medicine").Bold();
                            h.Cell().Text("Dosage").Bold();
                            h.Cell().Text("Duration").Bold();
                            h.Cell().Text("Times").Bold();
                        });

                        foreach (var m in _model.Medicines)
                        {
                            table.Cell().Text(m.MedicineName ?? "-");
                            table.Cell().Text(m.Dosage ?? "-");
                            table.Cell().Text(m.Duration ?? "-");
                            table.Cell().Text(m.TimesPerDay?.ToString() ?? "0");
                        }
                    });
                });
            });
        }
    }
}