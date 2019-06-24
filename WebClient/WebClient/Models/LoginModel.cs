using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Enter login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
