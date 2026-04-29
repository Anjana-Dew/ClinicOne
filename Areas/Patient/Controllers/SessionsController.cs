using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class SessionsController : BaseController
    {

        public SessionsController(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<IActionResult> Index()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            var sessions = await _context.ClinicSchedules
                .Where(x => x.PatientNIC == nic)
                .Include(x => x.ClinicSession)
                .OrderBy(x => x.ClinicDate)
                .ToListAsync();

            var now = DateTime.Now;

            var mapped = sessions.Select(s =>
            {
                string status;

                if (s.ClinicDate.Date > now.Date)
                    status = "Upcoming";
                else if (s.ClinicDate.Date == now.Date)
                    status = "Today";
                else
                    status = "Completed";

                return new PatientSessionViewModel
                {
                    ScheduleID = s.ScheduleID,
                    SessionName = s.ClinicSession.SessionName,
                    ClinicDate = s.ClinicDate,
                    StartTime = s.ClinicSession.StartTime,
                    EndTime = s.ClinicSession.EndTime,
                    Status = status
                };
            }).ToList();

            ViewBag.Upcoming = mapped.Where(x => x.Status == "Upcoming" || x.Status == "Today")
                                     .OrderBy(x => x.ClinicDate)
                                     .ToList();

            ViewBag.Past = mapped.Where(x => x.Status == "Completed")
                                 .OrderByDescending(x => x.ClinicDate)
                                 .ToList();

            ViewBag.NextSession = mapped
                .Where(x => x.Status == "Upcoming" || x.Status == "Today")
                .OrderBy(x => x.ClinicDate)
                .FirstOrDefault();

            return View(mapped);
        }
    }

}
