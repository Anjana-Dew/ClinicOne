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
        public int ParameterID { get; set; }

        [Required]
        public decimal TestValue { get; set; }

        [MaxLength(20)]
        public string ResultStatus { get; set; }


        [ForeignKey("ReportID")]
        public MedicalReport MedicalReport { get; set; }
        [ForeignKey("ParameterID")]
        public TestParameter TestParameter { get; set; }

    }
}
