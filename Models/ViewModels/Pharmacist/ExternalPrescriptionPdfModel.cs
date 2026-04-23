using System.Collections.Generic;

namespace ClinicOne.Models.ViewModels.Pharmacist
{

        public class ExternalPrescriptionPdfModel
        {
            public string PatientName { get; set; } = "";
            public string NIC { get; set; } = "";

            public List<MedicineVM> Medicines { get; set; } = new();
        }
    }