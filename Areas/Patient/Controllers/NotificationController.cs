using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Patient;
using ClinicOne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public NotificationController(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            await DetectAndCreate(nic);

            var notifications = await _context.Notifications
                .Where(n => n.PatientNIC == nic)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();

            var viewModel = notifications
                .Select(n => NotificationViewModel.From(n))
                .ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
                return Json(0);

            DetectAndCreate(nic).Wait();

            var count = _context.Notifications
                .Count(n => n.PatientNIC == nic && !n.IsRead);

            return Json(count);
        }

        [HttpPost]
        public IActionResult MarkRead(int id)
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            var notification = _context.Notifications
                .FirstOrDefault(n => n.NotificationID == id && n.PatientNIC == nic);

            if (notification == null)
                return Json(new { success = false });

            notification.IsRead = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult MarkAllRead()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
                return Json(new { success = false });

            var unread = _context.Notifications
                .Where(n => n.PatientNIC == nic && !n.IsRead)
                .ToList();

            foreach (var n in unread)
                n.IsRead = true;

            _context.SaveChanges();

            return Json(new { success = true, markedCount = unread.Count });
        }

        private async Task DetectAndCreate(string nic)
        {
            var existing = await _context.Notifications
                .Where(n => n.PatientNIC == nic)
                .Select(n => n.Message)
                .ToListAsync();

            var prescriptions = await _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .Include(p => p.PrescriptionMedicines)
                .OrderBy(p => p.PrescriptionDate)
                .ToListAsync();

            foreach (var prescription in prescriptions)
            {
                var marker = $"[PrescriptionID:{prescription.PrescriptionID}]";

                if (existing.Any(m => m.Contains(marker)))
                    continue;

                var names = prescription.PrescriptionMedicines
                    .Where(m => !string.IsNullOrEmpty(m.MedicineName))
                    .Select(m => m.MedicineName)
                    .ToList();

                var medicineList = names.Any() ? string.Join(", ", names) : "medicines";

                DateTime? endDate = null;
                foreach (var med in prescription.PrescriptionMedicines)
                {
                    var candidate = ParseEndDate(med.Duration);
                    if (candidate.HasValue && (endDate == null || candidate > endDate))
                        endDate = candidate;
                }

                var datePart = endDate.HasValue
                    ? $"Prescription|{endDate.Value:yyyy-MM-dd}"
                    : "Prescription";

                var message = $"[{datePart}] {marker} New prescription issued on {prescription.PrescriptionDate:dd MMM yyyy}. Medicines: {medicineList}.";

                _notificationService.SaveDirect(nic, message);
                existing.Add(message);
            }

            var schedules = await _context.ClinicSchedules
                .Where(s => s.PatientNIC == nic)
                .Include(s => s.ClinicSession)
                .OrderBy(s => s.ClinicDate)
                .ToListAsync();

            foreach (var schedule in schedules)
            {
                var marker = $"[ScheduleID:{schedule.ScheduleID}]";

                if (existing.Any(m => m.Contains(marker)))
                    continue;

                var sessionName = schedule.ClinicSession?.SessionName ?? "Clinic";
                var message = $"[Appointment|{schedule.ClinicDate:yyyy-MM-dd}] {marker} Your clinic appointment has been scheduled: {sessionName} on {schedule.ClinicDate:dddd, dd MMMM yyyy}.";

                _notificationService.SaveDirectWithSchedule(nic, schedule.ScheduleID, message);
                existing.Add(message);
            }

            var reports = await _context.MedicalReports
                .Where(r => r.PatientNIC == nic)
                .OrderBy(r => r.UploadedDate)
                .ToListAsync();

            foreach (var report in reports)
            {
                var marker = $"[ReportID:{report.ReportID}]";

                if (existing.Any(m => m.Contains(marker)))
                    continue;

                var message = $"[Report|{report.UploadedDate:yyyy-MM-dd}] {marker} A new medical report dated {report.UploadedDate:dd MMMM yyyy} has been uploaded to your profile.";

                _notificationService.SaveDirect(nic, message);
                existing.Add(message);
            }

            var notes = await _context.PatientProgresses
                .Where(p => p.PatientNIC == nic && p.IsConfirmed)
                .OrderBy(p => p.RecordedDate)
                .ToListAsync();

            foreach (var note in notes)
            {
                var marker = $"[NoteID:{note.ProgressID}]";

                if (existing.Any(m => m.Contains(marker)))
                    continue;

                var snippet = string.IsNullOrEmpty(note.DoctorNotes)
                    ? "No additional notes."
                    : (note.DoctorNotes.Length > 80 ? note.DoctorNotes[..80] + "…" : note.DoctorNotes);

                var message = $"[Note] {marker} Your doctor added a progress note. Status: {note.ProgressStatus}. Note: {snippet}";

                _notificationService.SaveDirect(nic, message);
                existing.Add(message);
            }

            var reminders = await _context.MedicineReminders
            .Include(r => r.PrescriptionMedicine)
            .Where(r => r.PatientNIC == nic && r.IsActive)
            .ToListAsync();

            foreach (var r in reminders)
            {
                if (DateTime.Today > r.EndDate)
                {
                    r.IsActive = false;
                    continue;
                }

                var marker = $"[ReminderID:{r.ReminderID}]";

                if (existing.Any(m => m.Contains(marker)))
                    continue;

                var med = r.PrescriptionMedicine?.MedicineName ?? "medicine";

                var message =
                    $"[Pharmacy|{r.EndDate:yyyy-MM-dd}] {marker} " +
                    $"Take your medicine: {med} daily until {r.EndDate:dd MMM yyyy}.";

                _notificationService.SaveDirectWithSchedule(nic, 0, message);
                existing.Add(message);
            }
        }

        private static DateTime? ParseEndDate(string duration)
        {
            if (string.IsNullOrEmpty(duration)) return null;

            var parts = duration.Trim().Split(' ');
            if (parts.Length != 2) return null;
            if (!int.TryParse(parts[0], out int value)) return null;

            var unit = parts[1].ToLower();

            if (unit.StartsWith("day")) return DateTime.Today.AddDays(value);
            if (unit.StartsWith("week")) return DateTime.Today.AddDays(value * 7);
            if (unit.StartsWith("month")) return DateTime.Today.AddMonths(value);

            return null;
        }
    }
}