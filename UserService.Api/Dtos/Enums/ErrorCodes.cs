namespace UserService.Dtos.Enums
{
    public enum ErrorCodes
    {
        Success = 10000,
        UnknownError = 10001,
        EmailAlreadyInUse = 10003,
        UserNotFound = 10004,
        WrongPassword = 10005
    }
}