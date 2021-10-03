using System;
using System.Collections.Generic;
using System.Text;

namespace pw.Commons.Models
{
    public class EnumCollection
    {
        public enum LanguageEnum
        { 
            English, Nepali
        }
        public enum SortOrderEnum
        { 
            Asc, Desc
        }

        public enum TblLogTypeEnum
        { 
            Error = 1,
            Info = 2,
            Auto = 3
        }
    }
}
