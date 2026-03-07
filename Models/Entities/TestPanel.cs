using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    [Table("TestPanel")]
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
