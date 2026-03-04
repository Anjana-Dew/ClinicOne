using ClinicOne.Data;
using ClinicOne.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DoctorDutyScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorDutyScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = new DoctorDutyScheduleViewModel
            {
                Doctors = _context.Doctors.Where(d =>d.IsActive).Select(d => new DoctorSelectViewModel
                {
                    DoctorID = d.DoctorID,
                    FullName = d.FullName
                }).ToList(),

                Sessions = _context.ClinicSessions.Select(s => new ClinicSessionSelectViewModel
                {
                    SessionID = s.SessionID,
                    SessionName = s.SessionName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveDoctors([FromBody] SaveDoctorDutyRequestViewModel request)
        {
            if (request.DoctorIds == null || !request.DoctorIds.Any())
            {
                return Json(new { success = false });
            }

            foreach (var doctorId in request.DoctorIds)
            {
                bool exists = _context.DoctorDutySchedules.Any(x =>
                    x.DoctorID == doctorId &&
                    x.SessionID == request.SessionId &&
                    x.ClinicDate == request.ClinicDate);

                if (!exists)
                {
                    _context.DoctorDutySchedules.Add(new Models.Entities.DoctorDutySchedule
                    {
                        DoctorID = doctorId,
                        SessionID = request.SessionId,
                        ClinicDate = request.ClinicDate
                    });
                }
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
