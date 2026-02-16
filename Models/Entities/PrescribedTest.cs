using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class PrescribedTest
    {
        [Key]
        public int PrescribedTestID { get; set; }

        [Required]
        public int TestTypeID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TestCategory { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int PrescriptionID { get; set; }

        [MaxLength(300)]
        public string Notes { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }


        [ForeignKey("TestTypeID")]
        public TestType TestType { get; set; }

        [ForeignKey("PrescriptionID")]
        public Prescription Prescription { get; set; }
    }
}
