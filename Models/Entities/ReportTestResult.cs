using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class ReportTestResult
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int ReportID { get; set; }

        [Required]
        public int TestTypeID { get; set; }

        [Required]
        public decimal TestValue { get; set; }

        [MaxLength(20)]
        public string ResultStatus { get; set; }


        [ForeignKey("ReportID")]
        public MedicalReport MedicalReport { get; set; }
        [ForeignKey("TestTypeID")]
        public TestType TestType { get; set; }

    }
}
