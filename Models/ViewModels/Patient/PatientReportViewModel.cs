using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientReportParameter
    {
        public string? ParameterName { get; set; }
        public string? TestValue { get; set; }
        public string? ResultStatus { get; set; }
    }

    public class PatientReportViewModel
    {
        public int ReportID { get; set; }
        public string? TestName { get; set; }
        public DateTime ReportDate { get; set; }
        public string? ReportPath { get; set; }
        public string? ReportStatus { get; set; }

        public List<PatientReportParameter> Parameters { get; set; } = new();
    }
}