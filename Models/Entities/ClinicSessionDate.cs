using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class ClinicSessionDate
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int SessionID { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [ForeignKey("SessionID")]
        public ClinicSession ClinicSession { get; set; }
    }
}
