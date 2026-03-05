using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class PrescribedTest
    {
        [Key]
        public int PrescribedTestID { get; set; }

        [Required]
        public int PanelID { get; set; }

        [Required]
        public int PrescriptionID { get; set; }

        public string TestCategory { get; set; }

        public DateTime OrderDate { get; set; }

        public string Notes { get; set; }

        public string Status { get; set; }

        [ForeignKey("PanelID")]
        public TestPanel TestPanel { get; set; }

        [ForeignKey("PrescriptionID")]
        public Prescription Prescription { get; set; }
    }
}
