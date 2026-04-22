using ClinicOne.Models.ViewModels.Pharmacist;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

                page.Header().Text("ClinicOne - External Prescription")
                    .FontSize(18).Bold().AlignCenter();

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Patient: {_model.PatientName}").FontSize(12);
                    col.Item().Text($"NIC: {_model.NIC}").FontSize(12);

                    col.Item().PaddingTop(10).Table(table =>
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
                        }
                    });
                });
            });
        }
    }
    }
