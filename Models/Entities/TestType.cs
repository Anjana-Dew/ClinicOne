using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.Entities
{
    public class TestType
    {
        [Key]
        public int TestTypeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TestName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(20)]
        public string Unit {  get; set; }

        public TestRange TestRange { get; set; }
        public ICollection<ReportTestResult> ReportTestResults { get; set; }
        public ICollection<PrescribedTest> PrescribedTests { get; set; }

    }
}
