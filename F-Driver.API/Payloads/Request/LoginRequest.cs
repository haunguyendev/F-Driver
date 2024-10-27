using F_Driver.API.Validation;
using System.ComponentModel.DataAnnotations;

namespace F_Driver.API.Payloads.Request
{
    public class LoginRequest
    {
        
        public string? IdToken { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        [Required]
        [RoleValidator]
        public string Role { get; set; }
    }
}
