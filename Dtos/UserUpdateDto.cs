using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos
{
    public class UserUpdateDto
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string PPLink { get; set; }
        public bool IsActive { get; set; }

    }
}