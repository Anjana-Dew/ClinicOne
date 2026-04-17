using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ClinicSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClinicSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //var sessions = _context.ClinicSessions
            //    .Include(s => s.SessionDates)
            //    .Select(s => new ClinicSessionViewModel
            //    {
            //        SessionID = s.SessionID,
            //        SessionName = s.SessionName,
            //        StartTime = s.StartTime,
            //        EndTime = s.EndTime,
            //        MaxSlots = s.MaxSlots,
            //        ScheduleType = s.ScheduleType,
            //        DaysOfWeek = s.ScheduleType == "Weekly" && s.DaysOfWeek != null ? s.DaysOfWeek.Split(',').ToList() : null,
            //        CustomDate = s.SessionDates.Select(d => (DateTime?)d.SessionDate).FirstOrDefault()
            //    }).ToList();

            var viewModel = new ClinicSessionViewModel
            {
                ExistingSessions = GetSessions()
            };

            return View(viewModel);
        }

        //create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClinicSessionViewModel model)
        {
            if(model.StartTime >= model.EndTime)
            {
                ModelState.AddModelError("", "End time must be later than start time");
            }
            if(model.ScheduleType == "Weekly" && (model.DaysOfWeek == null || !model.DaysOfWeek.Any()))
            {
                ModelState.AddModelError("", "Please select at least one day for weekly schedule.");
            }
            if(model.ScheduleType == "Custom" && model.CustomDate == null)
            {
                ModelState.AddModelError("", "Please select a date for custom schedule.");
            }
            if(model.ScheduleType == "Custom" && model.CustomDate < DateTime.Today)
            {
                ModelState.AddModelError("", "You cannot select a past date.");
            }
            if (!ModelState.IsValid)
            {
                model.ExistingSessions = GetSessions();
                return View("Index", model);
            }

            var session = new ClinicSession
            {
                SessionName = model.SessionName,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MaxSlots = model.MaxSlots,
                ScheduleType = model.ScheduleType,
                DaysOfWeek = model.ScheduleType == "Weekly" ? string.Join(",", model.DaysOfWeek) : null
            };

            _context.ClinicSessions.Add(session);
            _context.SaveChanges();

            if(model.ScheduleType == "Custom")
            {
                var sessionDate = new ClinicSessionDate
                {
                    SessionID = session.SessionID,
                    SessionDate = model.CustomDate.Value
                };

                _context.ClinicSessionDates.Add(sessionDate);
                _context.SaveChanges();
            }
            TempData["SuccessMessage"] = $"Session '{session.SessionName}' was created successfully.";

            return RedirectToAction(nameof(Index));
        }

        //Get Edit request
        public IActionResult Edit(int id)
        {
            var session = _context.ClinicSessions.Find(id);
            if(session == null)
            {
                return NotFound();
            }

            var model = new ClinicSessionViewModel
            {
                SessionID = session.SessionID,
                SessionName = session.SessionName,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                MaxSlots = session.MaxSlots,
                ScheduleType = session.ScheduleType,
                DaysOfWeek = session.ScheduleType == "Weekly" && session.DaysOfWeek != null ?
                                session.DaysOfWeek.Split(',').ToList() : null,
                CustomDate = session.SessionDates
                .Select( d => (DateTime?)d.SessionDate)
                .FirstOrDefault()
            };

            return View(model);
        }
        //Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ClinicSessionViewModel model) 
        {
            if(model.StartTime >= model.EndTime)
            {
                ModelState.AddModelError("", "End time must be later than start time.");
            }
            if (model.ScheduleType == "Weekly" && (model.DaysOfWeek == null || !model.DaysOfWeek.Any()))
            {
                ModelState.AddModelError("", "Please select at least one day.");
            }
            if (model.ScheduleType == "Custom" && model.CustomDate == null)
            {
                ModelState.AddModelError("", "Please select a date.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var session = _context.ClinicSessions
                .Include(s => s.SessionDates)
                .FirstOrDefault(s => s.SessionID == model.SessionID);

            if(session == null)
            {
                return NotFound();
            }

            var existingDate = session.SessionDates
                .Select(d => (DateTime?)d.SessionDate)
                .FirstOrDefault();

            if (session.ScheduleType == "Custom" && existingDate != null)
            {
                if (existingDate.Value.Date <= DateTime.Today)
                {
                    TempData["ErrorMessage"] = "You cannot edit a session that is today or in the past.";

                    return RedirectToAction(nameof(Index));
                }
            }

            session.SessionName = model.SessionName;
            session.StartTime = model.StartTime;
            session.EndTime = model.EndTime;
            session.MaxSlots = model.MaxSlots;
            session.ScheduleType = model.ScheduleType;

            if(model.ScheduleType == "Weekly")
            {
                session.DaysOfWeek = string.Join(",", model.DaysOfWeek);

                _context.ClinicSessionDates.RemoveRange(session.SessionDates);
            }
            else
            {
                session.DaysOfWeek = null;

                _context.ClinicSessionDates.RemoveRange(session.SessionDates);

                _context.ClinicSessionDates.Add(new ClinicSessionDate
                {
                    SessionID = session.SessionID,
                    SessionDate = model.CustomDate.Value
                });
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Session '{session.SessionName}' was updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        //Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var session = _context.ClinicSessions.Find(id);
            if(session == null)
            {
                return NotFound();
            }

            bool hasAssignments = _context.DoctorDutySchedules.Any(d => d.SessionID == id);

            if (hasAssignments)
            {
                TempData["ErrorMessage"] = $"The session '{session.SessionName}' cannot be deleted if it has past doctor assignments. Future sessions may be deleted only after removing assignments.";
                return RedirectToAction(nameof(Index));
            }
            

            _context.ClinicSessions.Remove(session);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Session '{session.SessionName}' was deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        //Get session list
        private List<ClinicSessionViewModel> GetSessions()
        {
            return _context.ClinicSessions.OrderByDescending(s => s.SessionID)
                .Select(s => new ClinicSessionViewModel
            {
                SessionID = s.SessionID,
                SessionName = s.SessionName,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxSlots = s.MaxSlots,
                ScheduleType = s.ScheduleType,
                DaysOfWeek = s.ScheduleType == "Weekly" && s.DaysOfWeek != null
                ? s.DaysOfWeek.Split(',').ToList(): null,
                CustomDate = s.SessionDates
                .Select(d => (DateTime?)d.SessionDate)
                .FirstOrDefault()

            }).ToList();
        }
    }
}
