using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos.Requests
{
    public class ApplicationUserUpdateDto
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string PPLink { get; set; }
        public bool IsActive { get; set; }

    }
}