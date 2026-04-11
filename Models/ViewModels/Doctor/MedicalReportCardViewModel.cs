namespace ClinicOne.Models.ViewModels.Doctor
{
    public class MedicalReportCardViewModel
    {
        public string PanelName { get; set; }


        public DateTime LatestUploadedDate {  get; set; }
        public string LatestReportPath { get; set; }
        public List<ParameterResultViewModel> LatestResults { get; set; }


        public DateTime? PreviousUploadedDate {  get; set; }
        public string PreviousReportPath { get; set; }
        public List<ParameterResultViewModel> PreviousResults { get; set; }
    }
}
