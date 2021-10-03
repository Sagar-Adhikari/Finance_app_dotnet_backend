using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.API.Views
{
    public class TaskView
    {
        public int SN { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Remarks { get; set; }
        public bool IsDeny { get; set; }
        public int OrdId { get; set; }

    }
    
}
