using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class MedicineHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicineHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string nic, int? month, int? year)
        {
            var prescriptionsQuery = _context.Prescriptions
                .Where(p => p.PatientNIC == nic);

            if (month.HasValue) 
            {
                prescriptionsQuery = prescriptionsQuery
                    .Where(p => p.PrescriptionDate.Month == month);
            }
            if (year.HasValue) 
            { 
                prescriptionsQuery = prescriptionsQuery
                    .Where(p => p.PrescriptionDate.Year == year);
            }
            var prescriptions = prescriptionsQuery
                .OrderByDescending(p => p.PrescriptionDate)
                .ThenByDescending(p => p.PrescriptionID)
                .ToList();
            var cards = new List<MedicineHistoryCardViewModel>();

            foreach(var pres in prescriptions)
            {
                var meds = _context.PrescriptionMedicines
                    .Where(m => m.PrescriptionID == pres.PrescriptionID)
                    .Select(m => new MedicineItemViewModel
                    {
                        MedicineName = m.MedicineName,
                        Dosage = m.Dosage,
                        TimesPerDay = m.TimesPerDay,
                        Duration = m.Duration
                    }).ToList();
                if (!meds.Any())
                {
                    continue;
                }
                var nextClinic = _context.ClinicSchedules
                    .Where(c => c.PatientNIC == nic && c.ClinicDate > pres.PrescriptionDate)
                    .OrderBy(c => c.ClinicDate)
                    .FirstOrDefault();

                cards.Add(new MedicineHistoryCardViewModel
                {
                    PrescriptionDate = pres.PrescriptionDate,
                    UntilDate = nextClinic?.ClinicDate,
                    Medicines = meds
                });
            }

            var years = _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .Select(p => p.PrescriptionDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            var model = new MedicineHistoryPageViewModel
            {
                PatientNIC = nic,
                SelectedMonth = month,
                SelectedYear = year,
                AvailableYears = years,
                MedicineHistories = cards
            };
            return View(model);
        }
    }
}
