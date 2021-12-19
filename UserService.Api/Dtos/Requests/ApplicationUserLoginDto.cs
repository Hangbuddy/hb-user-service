using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos.Requests
{
    public class ApplicationUserLoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}