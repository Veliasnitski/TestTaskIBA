using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Enter you login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Enter you password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password incorrect")]
        public string ConfirmPassword { get; set; }
    }
}
