using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class Doctor
    {
        [Key]
        public int DoctorID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(70)]
        public string Specialization { get; set; }

        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int UserAccountID { get; set; }

        [ForeignKey("UserAccountID")]
        public UserAccount UserAccount { get; set; }
        public ICollection<DoctorSchedule> DoctorSchedules { get; set; }
        public ICollection<AccessLog>  AccessLogs { get; set; }
    }
}
