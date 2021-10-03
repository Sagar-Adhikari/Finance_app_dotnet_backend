using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Commons.Extensions
{
    public static class Strings
    {
        public static string RemoveAllNonPrintableCharacters(string target)
        {
            return Regex.Replace(target, @"\p{C}+", string.Empty);
        }
    }
}
