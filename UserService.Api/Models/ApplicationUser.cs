using System;

namespace UserService.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}