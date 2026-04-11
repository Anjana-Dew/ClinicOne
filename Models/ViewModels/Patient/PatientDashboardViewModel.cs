namespace ClinicOne.Models.ViewModels.Patient;

public class PatientDashboardViewModel
{
    public string PatientName { get; set; }

    public string Gender { get; set; }

    public int Age { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    public decimal BMI { get; set; }

    public string BloodPressure { get; set; }

    public string ProgressStatus { get; set; }

    public string DoctorNotes { get; set; }
    public string BloodType { get; set; }

    public DateTime NextSessionDate { get; set; }

    public string LatestReportName { get; set; }

    public string ReportStatus { get; set; }

    public string ReportPath { get; set; }

    public List<MedicineDto> Medicines { get; set; }
}

public class MedicineDto
{
    public string Name { get; set; }

    public string Dosage { get; set; }
}

