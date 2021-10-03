using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Views
{
    public class PaginationView<T>
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public List<T> Contents { get; set; }
    }
}
