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
                    SessionName = s.SessionName,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToList(),

                ExistingSchedules = _context.DoctorDutySchedules.Select(x => new DoctorDutyItemViewModel
                {
                    DoctorID = x.DoctorID,
                    SessionID = x.SessionID,
                    ClinicDate = x.ClinicDate,
                    DoctorName = x.Doctor.FullName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveDoctors([FromBody] SaveDoctorDutyRequestViewModel request)
        {
            if(request.ClinicDate.Date < DateTime.Today)
            {
                return Json(new { success = false });
            }
            if (request.DoctorIds == null || !request.DoctorIds.Any())
            {
                return Json(new { success = false });
            }

            foreach (var doctorId in request.DoctorIds)
            {
                var dateOnly = request.ClinicDate.Date;

                bool exists = _context.DoctorDutySchedules.Any(x =>
                    x.DoctorID == doctorId &&
                    x.SessionID == request.SessionId &&
                    x.ClinicDate.Date == dateOnly);

                if (!exists)
                {
                    _context.DoctorDutySchedules.Add(new Models.Entities.DoctorDutySchedule
                    {
                        DoctorID = doctorId,
                        SessionID = request.SessionId,
                        ClinicDate = dateOnly
                    });
                }
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult RemoveDoctor([FromBody] RemoveDoctorDutyRequestViewModel request)
        {
            if (request.ClinicDate.Date < DateTime.Today)
            {
                return Json(new { success = false });
            }

            var dateOnly = request.ClinicDate.Date;

            var record = _context.DoctorDutySchedules.FirstOrDefault(x =>
                        x.DoctorID == request.DoctorID &&
                        x.SessionID == request.SessionID &&
                        x.ClinicDate.Date == dateOnly);

            if (record == null)
            {
                return Json(new { success = false });
            }

            _context.DoctorDutySchedules.Remove(record);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
