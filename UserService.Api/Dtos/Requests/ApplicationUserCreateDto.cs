using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos.Requests
{
    public class ApplicationUserCreateDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string DisplayName { get; set; }

    }
}