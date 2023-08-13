using Dots.Commons.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dots.Auth.DTO.Auth
{
    public class SignUpDTO
    {
        public bool Valid => 
            PasswordUtils.PasswordValid(Password) && 
            !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Email);
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
