namespace ClinicOne.Models.ViewModels.Patient
{
    public class NotificationVM
    {
        public int NotificationID { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
    }
}
