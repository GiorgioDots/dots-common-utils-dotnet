using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dots.Auth.DTO.Auth
{
    public class LoginDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Valid => !string.IsNullOrEmpty(Login) && 
            !string.IsNullOrEmpty(Password);
    }
}
