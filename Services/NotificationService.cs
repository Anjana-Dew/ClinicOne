using ClinicOne.Data;
using ClinicOne.Models.Entities;

namespace ClinicOne.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PharmacistDispenseSummary> NotifyPrescriptionProcessed(
            string patientNIC,
            List<PrescriptionMedicine> medicines)
        {
            var given = medicines
                .Where(m => m.Status == "Given")
                .Select(m => m.MedicineName)
                .ToList();

            var notGiven = medicines
                .Where(m => m.Status != "Given")
                .Select(m => $"{m.MedicineName} ({m.Reason ?? "no reason given"})")
                .ToList();

            var parts = new List<string>();
            if (given.Any()) parts.Add($"Given: {string.Join(", ", given)}");
            if (notGiven.Any()) parts.Add($"Not given: {string.Join(", ", notGiven)}");

            var message = $"[Pharmacy] Your prescription has been processed. {string.Join(". ", parts)}.";

            SaveDirect(patientNIC, message);

            return medicines.Select(m => new PharmacistDispenseSummary
            {
                MedicineName = m.MedicineName,
                Status = m.Status,
                Reason = m.Reason
            }).ToList();
        }

        public void NotifyExternalPdfReady(string patientNIC)
        {
            var message = "[Pharmacy] An external prescription PDF has been generated for you and is ready to view.";
            SaveDirect(patientNIC, message);
        }

        public void SaveDirect(string patientNIC, string message)
        {
            var scheduleID = _context.ClinicSchedules
                .Where(s => s.PatientNIC == patientNIC)
                .OrderByDescending(s => s.ScheduleID)
                .Select(s => (int?)s.ScheduleID)
                .FirstOrDefault();

            if (scheduleID == null) return;

            SaveWithSchedule(patientNIC, scheduleID.Value, message);
        }

        public void SaveDirectWithSchedule(string patientNIC, int scheduleID, string message)
        {
            SaveWithSchedule(patientNIC, scheduleID, message);
        }


        private void SaveWithSchedule(string patientNIC, int scheduleID, string message)
        {
            _context.Notifications.Add(new Notification
            {
                PatientNIC = patientNIC,
                ScheduleID = scheduleID,
                Message = message,
                SentDate = DateTime.Now,
                IsRead = false
            });

            _context.SaveChanges();
        }

        public static (string Type, DateTime? ReferenceDate, string Text) ParseMessage(string raw)
        {
            if (string.IsNullOrEmpty(raw) || !raw.StartsWith("["))
                return ("General", null, raw ?? "");

            var closingBracket = raw.IndexOf(']');
            if (closingBracket < 0)
                return ("General", null, raw);

            var prefix = raw[1..closingBracket];
            var rest = raw.Length > closingBracket + 2 ? raw[(closingBracket + 2)..] : "";

            var cleanText = System.Text.RegularExpressions.Regex.Replace(rest, @"\[[^\]]*ID:\d+\]\s*", "").Trim();

            var parts = prefix.Split('|');
            var type = parts[0];

            DateTime? refDate = null;
            if (parts.Length == 2 && DateTime.TryParse(parts[1], out var parsed))
                refDate = parsed;

            return (type, refDate, cleanText);
        }
    }

    public class PharmacistDispenseSummary
    {
        public string MedicineName { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}