using NepaliDateConverter;
using NepaliDateConverter.Helper;
using System;

namespace pw.Commons.Utils
{
    public static class NepaliDateHelper
    {
        public static FullDate GetFullNepaliDate(DateTime dt)
        {
            var ndt = new DateConverter().EngToNep(dt.Year, dt.Month, dt.Day);
            return ndt;
        }

        public static string GetNepaliDate(DateTime dt)
        {
            var ndt = new DateConverter().EngToNep(dt.Year, dt.Month, dt.Day);
            return $"{ ndt.ConvertedDate.Year}/{ndt.ConvertedDate.Month.ToString().PadLeft(2, '0')}/{ndt.ConvertedDate.Day.ToString().PadLeft(2, '0')}";
        }

        public static string GetNepaliYear(DateTime dt)
        {
            var ndt = new DateConverter().EngToNep(dt.Year, dt.Month, dt.Day);
            return $"{ndt.ConvertedDate.Year}";
        }

        public static int GetYear(DateTime engDate)
        {
            var fullDate = GetFullNepaliDate(engDate);
            return fullDate.ConvertedDate.Year;
        }
    }
}
