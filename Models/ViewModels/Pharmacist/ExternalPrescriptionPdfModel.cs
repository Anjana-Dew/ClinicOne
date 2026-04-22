using System.Collections.Generic;

namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class ExternalPrescriptionPdfModel
    {
        public string PatientName { get; set; } = "";
        public string NIC { get; set; } = "";
<<<<<<< HEAD
=======
        public string Notes { get; set; } = "";

>>>>>>> main
        public List<MedicineVM> Medicines { get; set; } = new();
    }
}