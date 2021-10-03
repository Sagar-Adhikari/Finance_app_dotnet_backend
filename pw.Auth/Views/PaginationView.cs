using System.Collections.Generic;

namespace pw.Auth.Views
{
    public class PaginationView<T>
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public ICollection<T> Contents { get; set; }
    }
}
