using QuestPDF.Fluent;
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
                page.Margin(40);

                page.Header()
                    .Text("ClinicOne Pharmacy - External Prescription")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Patient: {_model.PatientName}");
                    col.Item().Text($"NIC: {_model.NIC}");
                    col.Item().Text($"Date: {System.DateTime.Now:yyyy-MM-dd HH:mm}");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

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
                        }
                    });
                });
            });
        }
    }
}