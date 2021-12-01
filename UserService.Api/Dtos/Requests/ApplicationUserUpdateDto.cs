using System.ComponentModel.DataAnnotations;

namespace UserService.Dtos.Requests
{
    public class ApplicationUserUpdateDto
    {
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }

    }
}