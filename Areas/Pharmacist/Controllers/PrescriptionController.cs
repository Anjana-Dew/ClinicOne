using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Pharmacist;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    [Area("Pharmacist")]
    public class PrescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GenerateExternalPrescription(string nic)
        {
            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientNIC == nic);

            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefault();

            if (patient == null || prescription == null)
            {
                return Content("No prescription found.");
            }

            var medicines = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescription.PrescriptionID &&
                            m.Status == "Not Given")
                .Select(m => new MedicineItemViewModel
                {
                    MedicineName = m.MedicineName ?? "",
                    Dosage = m.Dosage ?? "",
                    Duration = m.Duration ?? "",
                    TimesPerDay = m.TimesPerDay
                })

                .ToList();

            var model = new ExternalPrescriptionPdfModel
            {
                PatientName = patient.FullName,
                NIC = patient.PatientNIC,
                Notes = prescription.Notes ?? " ",
                Medicines = medicines
            };

            return View("ExternalPrescription", model);
        }
    }
}