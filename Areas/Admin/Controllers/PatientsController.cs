using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Admin;
using ClinicOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterPatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                //{
                //    Console.WriteLine(error.ErrorMessage);
                //}
                return View("Index", model);
            }

            //NIC Validation
            if(!IsValidNIC(model.NIC, model.DOB.Value))
            {
                ModelState.AddModelError("NIC", "Invalid NIC format or NIC does not match Date of Birth.");
                return View("Index", model);
            }
            //Check NIC already exists
            if (_context.Patients.Any(p => p.PatientNIC == model.NIC))
            {
                ModelState.AddModelError("", "Patient with this NIC already exists.");
                return View("Index", model);
            }

            //Generate Password
            string last4 = model.NIC.Length >= 4 ? model.NIC.Substring(model.NIC.Length - 4) : model.NIC;

            string generatedPassword = $"Clinic@{last4}";

            string hashedPassword = PasswordService.HashPassword(generatedPassword);

            //Vital validation
            int? systolic = null;
            int? diastolic = null;

            if (!string.IsNullOrWhiteSpace(model.BloodPressure))
            {
                if (!model.BloodPressure.Contains("/"))
                {
                    ModelState.AddModelError("BloodPressure", "invalid format. Use 120/80");
                    return View("Index", model);
                }

                var parts = model.BloodPressure.Split('/');

                if (parts.Length != 2 || !int.TryParse(parts[0], out int sys) || !int.TryParse(parts[1], out int dia))
                {
                    ModelState.AddModelError("BloodPressure", "Invalid number in BP.");
                    return View("Index", model);
                }

                systolic = sys;
                diastolic = dia;
            }
            //create UserAccount
            var user = new UserAccount
            {
                Username = model.NIC,
                PasswordHash = hashedPassword,
                Role = "Patient",
                IsLocked = false,
                FailedAttempts = 0,
                FirstLogin = true
            };
            _context.UserAccounts.Add(user);
            _context.SaveChanges();


            
            //Create Patient 
            var patient = new ClinicOne.Models.Entities.Patient
            {
                PatientNIC = model.NIC,
                FullName = model.FullName,
                UserAccountID = user.UserAccountID,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DOB = model.DOB.Value,
                BloodType = model.BloodType,
                IsActive = true
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();

            //Vitals
            
           
            if (!string.IsNullOrEmpty(model.BloodPressure) && model.BloodPressure.Contains("/"))
            {
                var parts = model.BloodPressure.Split('/');

                if (parts.Length == 2)
                {
                    systolic = int.Parse(parts[0]);
                    diastolic = int.Parse(parts[1]);
                }
            }
            var vital = new PatientVital
            {
                PatientNIC = patient.PatientNIC,
                Height = model.Height,
                Weight = model.Weight,
                Systolic = systolic,
                Diastolic = diastolic
            };

            _context.PatientVitals.Add(vital);
            _context.SaveChanges();

            TempData["SuccessMessage"] =
                $"Patient register successfully. Default Password: {generatedPassword} ";

            return RedirectToAction("Index");
        }

        // NIC Validation Method
        private bool IsValidNIC(string nic, DateTime dob) {
            
            if(string.IsNullOrWhiteSpace(nic))  
                return false;

            nic= nic.Trim();
            
            //NIC's with 12 characters
            if(nic.Length == 12)
            {
                if (!nic.All(char.IsDigit))
                {
                    return false;
                }

                string yearPart = nic.Substring(0, 4);

                if (int.Parse(yearPart) != dob.Year)
                    { return false; }

                return true;
            }

            //NIC's with 10 characters
            if(nic.Length == 10)
            {
                string firstNine = nic.Substring(0, 9);
                char lastChar = nic[9];

                if (!firstNine.All(char.IsDigit))
                {
                    return false;
                }

                if(lastChar != 'v' &&  lastChar != 'V')
                {
                    return false; 
                }

                string yearPart = nic.Substring(0, 2);
                string birthYearLastTwo = dob.Year.ToString().Substring(2, 2);

                if (yearPart != birthYearLastTwo) {
                    return false;
                }
                return true;
            }

            return false;
        }

        // search patients
        [HttpGet]
        public IActionResult SearchPatient(string nic) 
        {
            if (string.IsNullOrWhiteSpace(nic))
            {
                return Json(new { success = false, message = "Please enter a NIC." });
            }

            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == nic);

            if (patient == null) 
            {
                return Json(new { success = false, message = "Patinet not found." });
            
            }
            return Json(new
            {
                success = true,
                fullName = patient.FullName,
                nic = patient.PatientNIC,
                isActive = patient.IsActive
            });
        }

        // deactivate patient profiels
        [HttpPost]
        public IActionResult DeactivatePatient(string nic) 
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == nic);

            if (patient == null) 
            {
                return Json(new { success = false });
            }

            patient.IsActive = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        //activate patients
        [HttpPost]
        public IActionResult ActivatePatient(string nic)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == nic);

            if(patient == null)
            {
                return Json(new { success = false });
            }

            patient.IsActive = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        //Bp parse helper methods
        //private int? ParseSystolic(string bp)
        //{
        //    if (string.IsNullOrEmpty(bp) || !bp.Contains("/")) return null;
        //    return int.Parse(bp.Split('/')[0]);
        //}
        //private int? ParseDiastolic(string bp)
        //{
        //    if (string.IsNullOrEmpty(bp) || !bp.Contains("/")) return null;
        //    return int.Parse(bp.Split('/')[1]);
        //}
    }
}
