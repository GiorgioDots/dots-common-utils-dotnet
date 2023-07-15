using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.DTO
{
    public enum eEntityStatus
    {
        OK = 255,
        DELETED = 100,
        DELETED_BY_REF = 101,
    }
}
