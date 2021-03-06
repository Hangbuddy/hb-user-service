using UserService.Dtos.Enums;

namespace UserService.Dtos.Responses
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public ErrorCodes ErrorCode { get; set; }
        public ApplicationUserReadDto User { get; set; }
    }
}