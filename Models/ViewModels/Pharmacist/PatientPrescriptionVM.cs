namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class PatientPrescriptionVM
    {
        public int PrescMedID { get; set; }

        public string? MedicineName { get; set; }   
        public string? Dosage { get; set; }         
        public string? Duration { get; set; }      
        public int? TimesPerDay { get; set; }      
        public string? Status { get; set; }        
        public string? Reason { get; set; }        

        public bool? PatientConfirmed { get; set; } 
    }
}
