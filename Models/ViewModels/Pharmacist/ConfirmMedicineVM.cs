namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class ConfirmMedicineVM
    {
        public int PrescMedID { get; set; }
        public string Status { get; set; } = "";
        public string? Reason { get; set; }
    }
}