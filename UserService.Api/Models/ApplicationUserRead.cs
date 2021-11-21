namespace UserService.Models
{
    public class ApplicationUserRead
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }
    }
}