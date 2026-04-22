<<<<<<< HEAD
﻿using ClinicOne.Models.ViewModels.Pharmacist;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
=======
﻿using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ClinicOne.Models.ViewModels.Pharmacist;
>>>>>>> main

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
                page.Margin(25);
                page.Size(PageSizes.A4);

<<<<<<< HEAD
                page.Header().Text("ClinicOne - External Prescription")
                    .FontSize(18).Bold().AlignCenter();
=======
                page.Header()
                    .Text("ClinicOne Pharmacy - External Prescription")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();
>>>>>>> main

                page.Content().Column(col =>
                {
                    col.Spacing(10);

<<<<<<< HEAD
                    col.Item().Text($"Patient: {_model.PatientName}").FontSize(12);
                    col.Item().Text($"NIC: {_model.NIC}").FontSize(12);

                    col.Item().PaddingTop(10).Table(table =>
=======
                    col.Item().Text($"Patient: {_model.PatientName}");
                    col.Item().Text($"NIC: {_model.NIC}");
                    col.Item().Text($"Date: {System.DateTime.Now:yyyy-MM-dd HH:mm}");

                    col.Item().Table(table =>
>>>>>>> main
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3); // medicine
                            c.RelativeColumn(2); // dosage
                            c.RelativeColumn(2); // duration
                            c.RelativeColumn(1); // times
                            c.RelativeColumn(2); // status
                            c.RelativeColumn(3); // reason
                        });

<<<<<<< HEAD
                        // HEADER
                        table.Header(h =>
                        {
                            h.Cell().Background("#2C3E50").Padding(5).Text("Medicine").FontColor("white");
                            h.Cell().Background("#2C3E50").Padding(5).Text("Dosage").FontColor("white");
                            h.Cell().Background("#2C3E50").Padding(5).Text("Duration").FontColor("white");
                            h.Cell().Background("#2C3E50").Padding(5).Text("Times").FontColor("white");
                            h.Cell().Background("#2C3E50").Padding(5).Text("Status").FontColor("white");
                            h.Cell().Background("#2C3E50").Padding(5).Text("Reason").FontColor("white");
                        });

                        foreach (var m in _model.Medicines)
                        {
                            table.Cell().Padding(5).Text(m.MedicineName ?? "-");
                            table.Cell().Padding(5).Text(m.Dosage ?? "-");
                            table.Cell().Padding(5).Text(m.Duration ?? "-");
                            table.Cell().Padding(5).Text(m.TimesPerDay.ToString());
                            table.Cell().Padding(5).Text(m.Status ?? "-");
                            table.Cell().Padding(5).Text(m.Reason ?? "-");
=======
                        table.Header(header =>
                        {
                            header.Cell().Text("Medicine").Bold();
                            header.Cell().Text("Dosage").Bold();
                            header.Cell().Text("Times/Day").Bold();
                            header.Cell().Text("Reason").Bold();
                        });

                        foreach (var med in _model.Medicines)
                        {
                            table.Cell().Text(med.MedicineName ?? "-");
                            table.Cell().Text(med.Dosage ?? "-");
                            table.Cell().Text(med.TimesPerDay.ToString());
                            table.Cell().Text(med.Reason ?? "-");
>>>>>>> main
                        }
                    });
                });
            });
        }
    }
<<<<<<< HEAD
    }
=======
}
>>>>>>> main
