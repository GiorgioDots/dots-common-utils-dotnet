using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.DTO.Users;

namespace UserManager.DTO.Tags
{
    public class TagDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public List<UserDTO> Users { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? DateUpdate { get; set; }
    }
}
