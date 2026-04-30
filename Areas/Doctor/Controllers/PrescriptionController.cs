using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Doctor;
using ClinicOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClinicOne.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class PrescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AccessLogService _accessLogService;

        public PrescriptionController(ApplicationDbContext context, AccessLogService accessLogService)
        {
            _context = context;
            _accessLogService = accessLogService;
        }

        public IActionResult Create(string nic)
        {
            var tests = _context.TestPanels.Select(t => new TestOptionViewModel
            {
                PanelID = t.PanelID,
                TestName = t.TestName,
            }).ToList();

            var model = new CreatePrescriptionViewModel
            {
                PatientNIC = nic,

                Medicines = new List<MedicineInputViewModel>
                {
                    new MedicineInputViewModel()
                },
                Tests = new List<TestRowViewModel>
                {
                    new TestRowViewModel()
                },
                AvailableTests = tests
            };
          
            return View(model);
        }

        [HttpPost]
        public IActionResult SavePrescription(CreatePrescriptionViewModel model)
        {
            foreach (var med in model.Medicines)
            {
                if (!string.IsNullOrEmpty(med.MedicineName))
                {
                    if (string.IsNullOrEmpty(med.Dosage) || med.TimesPerDay <= 0)
                    {
                        TempData["Error"] = "Dosage and Times/Day are required.";
                        return RedirectToAction("Create", new { nic = model.PatientNIC });
                    }

                    if (!med.DurationValue.HasValue || med.DurationValue <= 0 || string.IsNullOrEmpty(med.DurationUnit))
                    {
                        TempData["Error"] = "Duration is required.";
                        return RedirectToAction("Create", new { nic = model.PatientNIC });
                    }
                }
            }
            bool hasValidMedicine = model.Medicines.Any(m => !string.IsNullOrEmpty(m.MedicineName));
            bool hasValidTest = model.Tests != null && model.Tests.Any(t => t.PanelID > 0);

            if (!hasValidMedicine && !hasValidTest)
            {
                TempData["Error"] = "Prescription must contain at least a medicine or a test.";
                return RedirectToAction("Create", new { nic = model.PatientNIC });
            }
            var prescription = new Prescription
            {
                PatientNIC = model.PatientNIC,
                PrescriptionDate = DateTime.Now,
                Notes = model.Notes,
                IsCompleted = false
            };

            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();

            foreach(var med in model.Medicines)
            {
                string duration = null;

                if(med.DurationValue.HasValue && !string.IsNullOrEmpty(med.DurationUnit))
                {
                    duration = $"{med.DurationValue} {med.DurationUnit}";
                }
                var medicine = new PrescriptionMedicine
                {
                    PrescriptionID = prescription.PrescriptionID,
                    MedicineName = med.MedicineName,
                    Dosage = med.Dosage,
                    TimesPerDay = med.TimesPerDay,
                    Duration = duration,
                    Status = "Not Given"
                };

                _context.PrescriptionMedicines.Add(medicine);
            }

            if (model.Tests != null) 
            { 
                model.Tests = model.Tests.Where(t => t.PanelID > 0).ToList();

                foreach(var test in model.Tests)
                {
                    var prescribedTest = new PrescribedTest
                    {
                        PanelID = test.PanelID,
                        TestCategory = "Lab",
                        OrderDate = DateTime.Now,
                        PrescriptionID = prescription.PrescriptionID,
                        Notes = test.Notes,
                        Status = "Ordered"
                    };

                    _context.PrescribedTests.Add(prescribedTest);
                }
            }
            _context.SaveChanges();
            _accessLogService.Log(model.PatientNIC, "Prescribe");

            TempData["Success"] = "Prescription saved successfully";

            return RedirectToAction("Index", "PatientMedicalProfile", new { id = model.PatientNIC });
        }
    }
}
