using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class DevelopModel
    {
        [Required(ErrorMessage = "Enter login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Choise role")]
 
        public Role Role { get; set; }
    }
}
