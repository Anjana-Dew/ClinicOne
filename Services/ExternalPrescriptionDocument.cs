using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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

                page.Header().Text("ClinicOne - External Prescription")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Patient Name: {_model.PatientName}");
                    col.Item().Text($"NIC: {_model.NIC}");

                    col.Item().PaddingTop(10).Text("Medicines").Bold();

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
                            header.Cell().Text("Medicine");
                            header.Cell().Text("Dosage");
                            header.Cell().Text("Times/Day");
                            header.Cell().Text("Duration");
                        });

                        foreach (var med in _model.Medicines)
                        {
                            table.Cell().Text(med.MedicineName);
                            table.Cell().Text(med.Dosage);
                            table.Cell().Text(med.TimesPerDay.ToString());
                            table.Cell().Text(med.Duration);
                        }
                    });

                    col.Item().PaddingTop(15).Text($"Notes: {_model.Notes}");
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("ClinicOne Pharmacy System | Generated ");
                    text.Span(DateTime.Now.ToString("yyyy-MM-dd")).Bold();
                });
            });
        }
    }
}
