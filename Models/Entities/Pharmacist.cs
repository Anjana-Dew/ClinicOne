using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class Pharmacist
    {
        [Key]
        public int PharmacistID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; }

        [Required]
        public int UserAccountID { get; set; }

        [ForeignKey("UserAccountID")]
        public UserAccount UserAccount { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }
}
