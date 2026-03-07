using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{

    [Table("TestParameter")]
    public class TestParameter
    {
        [Key]
        public int ParameterID { get; set; }

        [Required]
        public int PanelID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ParameterName { get; set; }

        [MaxLength(20)]
        public string Unit { get; set; }

        [ForeignKey("PanelID")]
        public TestPanel TestPanel { get; set; }

        public ICollection<TestRange> TestRanges { get; set; }
        public ICollection<ReportTestResult> ReportTestResults { get; set; }
    }
}
