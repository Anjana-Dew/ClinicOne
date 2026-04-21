namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientNoteViewModel
    {
        public int ProgressID { get; set; }

        public string PatientNIC { get; set; }

        public DateTime ProgressDate { get; set; }

        public string ProgressStatus { get; set; }

        public bool IsConfirmed { get; set; }

        public string DoctorNotes { get; set; }
    }
}
