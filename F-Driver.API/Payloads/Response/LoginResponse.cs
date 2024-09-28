namespace F_Driver.API.Payloads.Response
{
    public class LoginResponse
    {
        
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

    }
}
