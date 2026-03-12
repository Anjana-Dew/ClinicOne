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

        //public IActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Index()
        {
            var sessions =  _context.ClinicSessions.Select(s => new ClinicSessionViewModel
            {
                SessionID = s.SessionID,
                SessionName = s.SessionName,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxSlots = s.MaxSlots
            }).ToList();

            var viewModel = new ClinicSessionViewModel
            {
                ExistingSessions = sessions
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
                MaxSlots = model.MaxSlots
            };

            _context.ClinicSessions.Add(session);
            _context.SaveChanges();

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
                MaxSlots = session.MaxSlots
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var session = _context.ClinicSessions.Find(model.SessionID);
            if(session == null)
            {
                return NotFound();
            }

            session.SessionName = model.SessionName;
            session.StartTime = model.StartTime;
            session.EndTime = model.EndTime;
            session.MaxSlots = model.MaxSlots;

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

            string sessionName = session.SessionName;

            _context.ClinicSessions.Remove(session);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Session '{session.SessionName}' was deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        //Get session list
        private List<ClinicSessionViewModel> GetSessions()
        {
            return _context.ClinicSessions.Select(s => new ClinicSessionViewModel
            {
                SessionID = s.SessionID,
                SessionName = s.SessionName,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MaxSlots = s.MaxSlots
            }).ToList();
        }
    }
}
