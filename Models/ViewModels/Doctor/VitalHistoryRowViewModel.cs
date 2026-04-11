namespace ClinicOne.Models.ViewModels.Doctor
{
    public class VitalHistoryRowViewModel
    {
        public DateTime RecordedDate { get; set; }

        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public int? Systolic { get; set; }
        public int? Diastolic { get; set; }

        public string BloodPressure => (Systolic != null && Diastolic != null) ? $"{Systolic}/ {Diastolic}" : "No value";
    }
}
