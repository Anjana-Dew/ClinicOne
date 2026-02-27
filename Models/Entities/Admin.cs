using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public int UserAccountID { get; set; }


        [ForeignKey("UserAccountID")]
        public UserAccount UserAccount { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
