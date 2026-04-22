namespace ClinicOne.Models.ViewModels.Patient
{
    public class ReportResultDto
    {
        public string TestName { get; set; }
        public string Parameter { get; set; }
        public decimal Value { get; set; }
        public string Status { get; set; }
    }
}
