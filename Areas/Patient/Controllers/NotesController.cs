using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class NotesController : BaseController
    {


        public NotesController(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<IActionResult> Index()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
            {
                var username = User.Identity?.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var patient = await _context.Patients
                        .Include(p => p.UserAccount)
                        .FirstOrDefaultAsync(p => p.UserAccount.Username == username);

                    if (patient != null)
                    {
                        nic = patient.PatientNIC;

                        HttpContext.Session.SetString("PatientNIC", nic);
                    }
                }
            }

            if (string.IsNullOrEmpty(nic))
            {
                return RedirectToAction("Login", "Account");
            }

            var notes = await _context.PatientProgresses
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.ProgressDate)
                .ThenByDescending(p => p.ProgressID)
                .Select(p => new PatientNoteViewModel
                {
                    ProgressID = p.ProgressID,
                    ProgressDate = p.ProgressDate,
                    ProgressStatus = p.ProgressStatus ?? "Stable",
                    DoctorNotes = p.DoctorNotes ?? "No notes provided",
                    IsConfirmed = p.IsConfirmed
                })
                .ToListAsync();

            return View(notes);
        }
    }
}