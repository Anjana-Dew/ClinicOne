using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.Entities
{
    public class TestPanel
    {
        [Key]
        public int PanelID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TestName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public ICollection<TestParameter> TestParameters { get; set; }
        public ICollection<PrescribedTest> PrescribedTests { get; set; }
    }
}
