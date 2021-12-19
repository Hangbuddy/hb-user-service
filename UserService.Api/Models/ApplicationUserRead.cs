using System;

namespace UserService.Models
{
    public class ApplicationUserRead
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime BirthDate { get; set; }
    }
}