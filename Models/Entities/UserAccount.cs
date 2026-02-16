using AspNetCoreGeneratedDocument;
using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.Entities
{
    public class UserAccount
    {
        [Key]
        public int UserAccountID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        [Required]
        public int FailedAttempts { get; set; }
        public DateTime? LastLogin {  get; set; }

        public Patient patient { get; set; }
        public Doctor Doctor { get; set; }
        public Admin Admin { get; set; }
        public Pharmacist Pharmacist { get; set; }

    }
}
