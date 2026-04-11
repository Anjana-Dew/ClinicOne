namespace ClinicOne.Models.ViewModels.Doctor
{
    public class BPUpdateRequest
    {
        public string Nic { get; set; }
        public string Bp { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
    }
}
