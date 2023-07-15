using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.DTO.Users;

namespace UserManager.DTO.Tags
{
    public class UserTagDTO
    {
        public int Id { get; set; }

        public int IdUser { get; set; }

        public int IdTag { get; set; }

        public int Status { get; set; }

        public DateTime? DateCreate { get; set; }

        public DateTime? DateUpdate { get; set; }

        public TagDTO IdTagNavigation { get; set; }

        public UserDTO IdUserNavigation { get; set; }

    }
}
