namespace ClinicOne.Models.ViewModels
{
    public class ClinicSessionSelectViewModel
    {
        public int SessionID { get; set; }
        public string SessionName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
