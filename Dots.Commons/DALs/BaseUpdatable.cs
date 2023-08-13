using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dots.Commons.DALs
{
    public class BaseUpdatable
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public virtual async Task DeleteReferences(object dc)
        {
            return;
        }
    }
}
