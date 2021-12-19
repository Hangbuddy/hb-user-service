using System;

namespace UserService.Dtos.Requests
{
    public class ApplicationUserUpdateDto
    {
        public string Password { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime BirthDate { get; set; }
    }
}