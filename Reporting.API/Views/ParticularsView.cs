
using System.Collections.Generic;

namespace Reporting.API.Views
{
    public class ParticularsView
    {
        public string Title { get; set; }
        public bool IsCredit { get; set; }
        public string Amount { get; set; }
        public List<ParticularsView> LsParticulars { get; set; }
    }

    public class NewParticularModel
    {
        public int AccID { get; set; }
        public int RelatedAccSN { get; set; }
        public string ParticuName { get; set; }
        public string NameInDev { get; set; }
        public bool DrLock { get; set; }
    }
}