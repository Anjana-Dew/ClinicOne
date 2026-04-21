namespace ClinicOne.Models.ViewModels.Patient;

public class PatientDashboardViewModel
{
    public string PatientName { get; set; } = "";
    public string NIC { get; set; } = "";
    public string BloodType { get; set; } = "";
    public string Address { get; set; } = "";
    public string PhoneNumber { get; set; } = "";

    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal BMI { get; set; }
    public string BloodPressure { get; set; } = "";

    public string ProgressStatus { get; set; } = "";
    public string DoctorNotes { get; set; } = "";

    public DateTime? NextSessionDate { get; set; }
    public string NextSessionName { get; set; } = "";
    public string SessionTime { get; set; } = "";

    public int? ReportID { get; set; }
    public string TestName { get; set; } = "";
    public string Result { get; set; } = "";
    public DateTime? ReportDate { get; set; }
    public string ReportStatus { get; set; } = "";
    public string ReportPath { get; set; } = "";

    public List<MedicineDto> Medicines { get; set; } = new();
}

public class MedicineDto
{
    public string Name { get; set; } = "";
    public string Dosage { get; set; } = "";
}
