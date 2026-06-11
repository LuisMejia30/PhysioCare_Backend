using System.ComponentModel.DataAnnotations;

namespace physiocare_backend.Models
{
    public class LoginDto
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}