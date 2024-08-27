using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Models.Login
{
    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
