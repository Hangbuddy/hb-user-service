using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos.Requests
{
    public class ApplicationUserCreateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}