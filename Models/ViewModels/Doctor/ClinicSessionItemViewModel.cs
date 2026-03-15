namespace ClinicOne.Models.ViewModels.Doctor
{
    public class ClinicSessionItemViewModel
    {
        public int SessionID { get; set; }
        public string SessionName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int RemainingSlots { get; set; }
    }
}
