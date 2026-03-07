using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Admin
{
    public class ClinicSessionViewModel
    {
        public int SessionID { get; set; }

        [Required(ErrorMessage = "Session name is required")]
        [StringLength(100)]
        public string SessionName { get; set; }

        [Required(ErrorMessage = " Start time is required")]
        public TimeSpan StartTime {  get; set; }

        [Required(ErrorMessage = " End time is required")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Max slots is required")]
        [Range(1,1000, ErrorMessage = "Max slots must be grater than 0")]
        public int MaxSlots { get; set; }

        public List<ClinicSessionViewModel>? ExistingSessions { get; set; }

    }
}
