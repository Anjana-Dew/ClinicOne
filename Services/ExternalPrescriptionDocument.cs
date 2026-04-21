using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Medicine");
                            header.Cell().Element(CellStyle).Text("Dosage");
                            header.Cell().Element(CellStyle).Text("Times/Day");
                            header.Cell().Element(CellStyle).Text("Duration");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).Padding(5);
                            }
                        });

                        // Rows
                        foreach (var med in _model.Medicines)
                        {
                            table.Cell().Element(CellStyle).Text(med.MedicineName);
                            table.Cell().Element(CellStyle).Text(med.Dosage);
                          
                            table.Cell().Element(CellStyle).Text(med.Duration);
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Padding(5);
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
