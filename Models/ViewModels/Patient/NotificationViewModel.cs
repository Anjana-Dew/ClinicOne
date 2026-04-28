using ClinicOne.Services;

namespace ClinicOne.Models.ViewModels.Patient
{
    public class NotificationViewModel
    {
        public int NotificationID { get; set; }
        public string PatientNIC { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }

        public string RawMessage { get; set; }


        public string Type { get; set; }

        public DateTime? ReferenceDate { get; set; }

        public string Message { get; set; }


        public int? DaysRemaining => ReferenceDate.HasValue
            ? (int)(ReferenceDate.Value.Date - DateTime.Today).TotalDays
            : null;

        public string RelativeTime
        {
            get
            {
                var diff = DateTime.Now - SentDate;

                if (diff.TotalMinutes < 1) return "Just now";
                if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min{((int)diff.TotalMinutes == 1 ? "" : "s")} ago";
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hour{((int)diff.TotalHours == 1 ? "" : "s")} ago";
                if (diff.TotalDays < 2) return "Yesterday";
                if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} days ago";

                return SentDate.ToString("dd MMM yyyy");
            }
        }

        public static NotificationViewModel From(Entities.Notification n)
        {
            var (type, refDate, text) = NotificationService.ParseMessage(n.Message);

            return new NotificationViewModel
            {
                NotificationID = n.NotificationID,
                PatientNIC = n.PatientNIC,
                SentDate = n.SentDate,
                IsRead = n.IsRead,
                RawMessage = n.Message,
                Type = type,
                ReferenceDate = refDate,
                Message = text
            };
        }
    }
}
