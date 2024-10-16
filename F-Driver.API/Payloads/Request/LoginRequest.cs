namespace F_Driver.API.Payloads.Request
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
