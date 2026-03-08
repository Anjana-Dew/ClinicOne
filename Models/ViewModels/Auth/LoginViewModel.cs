using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required]
        public string Username {  get; set; }
        [Required]
        public string Password { get; set; }
    }
}
