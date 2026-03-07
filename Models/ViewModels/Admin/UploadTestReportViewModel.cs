namespace ClinicOne.Models.ViewModels.Admin
{
    public class UploadTestReportViewModel
    {
        public string PatientNIC { get; set; }
        public string PatientName { get; set; }
        public List<TestPanelViewModel> Panels { get; set; }
    }
}
