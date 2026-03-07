namespace ClinicOne.Models.ViewModels.Admin
{
    public class SaveTestReportRequest
    {
        public string PatientNIC { get; set; }
        public int PanelID { get; set; }

        public Dictionary<int, decimal> TestValues { get; set; }
    }
}
