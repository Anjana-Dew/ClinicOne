using ClinicOne.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Update(
            string PatientNIC,
            string FullName,
            string PhoneNumber,
            string BloodType,
            string Address)
        {
            // ================= VALIDATION =================
            if (string.IsNullOrEmpty(PatientNIC))
                return Json(new { success = false, message = "Invalid request" });

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientNIC == PatientNIC);

            if (patient == null)
                return Json(new { success = false, message = "Patient not found" });

            
            // ================= UPDATE LOGIC =================
            bool updated = false;

            if (!string.IsNullOrEmpty(FullName) && patient.FullName != FullName)
            {
                patient.FullName = FullName;
                updated = true;
            }

            if (!string.IsNullOrEmpty(PhoneNumber) && patient.PhoneNumber != PhoneNumber)
            {
                patient.PhoneNumber = PhoneNumber;
                updated = true;
            }


            if (!string.IsNullOrEmpty(Address) && patient.Address != Address)
            {
                patient.Address = Address;
                updated = true;
            }

            if (!updated)
            {
                return Json(new
                {
                    success = false,
                    message = "No changes detected"
                });
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Profile updated successfully",
                data = new
                {
                    fullName = patient.FullName,
                    phoneNumber = patient.PhoneNumber,
                    address = patient.Address
                }
            });
        }
    }
}