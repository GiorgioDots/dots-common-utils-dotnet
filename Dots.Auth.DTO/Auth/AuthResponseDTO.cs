using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dots.Auth.DTO.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
