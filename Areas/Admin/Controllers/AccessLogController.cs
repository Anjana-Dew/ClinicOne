using ClinicOne.Data;
using ClinicOne.Models.ViewModels;
using ClinicOne.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccessLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccessLogController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Doctors = _context.Doctors.ToList();
            return View();
        }

        public IActionResult Results(AccessLogFilterViewModel filter)
        {
            var query = from log in _context.AccessLogs
                        join d in _context.Doctors on log.DoctorID equals d.DoctorID
                        join p in _context.Patients on log.PatientNIC equals p.PatientNIC
                        select new
                        {
                            log,
                            d,
                            p
                        };

            if (filter.Month.HasValue)
            {
                query = query.Where(x => x.log.AccessDateTime.Month == filter.Month.Value);
            }

            if (filter.Year.HasValue)
            {
                query = query.Where(x => x.log.AccessDateTime.Year == filter.Year.Value);
            }

            if (filter.DoctorID.HasValue)
            {
                query = query.Where(x => x.log.DoctorID == filter.DoctorID.Value);
            }

            if (!string.IsNullOrEmpty(filter.AccessAction))
            {
                query = query.Where(x => x.log.Action == filter.AccessAction);
            }

            if (!string.IsNullOrEmpty(filter.PatientNIC))
            {
                query = query.Where(x => x.log.PatientNIC == filter.PatientNIC);
            }

            var logs = query
                .OrderByDescending(x => x.log.AccessDateTime)
                .Select(x => new AccessLogViewModel
                {
                    AccessDateTime = x.log.AccessDateTime,
                    DoctorName = x.d.FullName,
                    RegistrationNumber = x.d.RegistrationNumber,
                    PatientName = x.p.FullName,
                    PatientNIC = x.p.PatientNIC,
                    AccessAction = x.log.Action
                })
                .ToList();

            ViewBag.Filters = filter;

            if (filter.DoctorID.HasValue)
            {
                var doctor = _context.Doctors
                    .Where(d => d.DoctorID == filter.DoctorID.Value)
                    .Select(d => new
                    {
                        d.FullName,
                        d.RegistrationNumber
                    }).FirstOrDefault();

                if (doctor != null)
                {
                    ViewBag.DoctorName = doctor.FullName;
                    ViewBag.DoctorReg = doctor.RegistrationNumber;
                }
            }
            return View(logs);
        }
    }
}
