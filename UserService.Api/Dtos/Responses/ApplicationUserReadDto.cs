using System;

namespace UserService.Dtos.Responses
{
    public class ApplicationUserReadDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public DateTime BirthDate { get; set; }

    }
}