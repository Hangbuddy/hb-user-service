using System;

namespace UserService.Dtos.Responses
{
    public class ApplicationUserReadDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string PPLink { get; set; }
        public bool IsActive { get; set; }
    }
}