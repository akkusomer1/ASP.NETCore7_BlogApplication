using System.Net.NetworkInformation;

namespace ProgrammersBlog.Mvc.Extantions
{
    public static class DateTimeExtantion
    {

        public static string FullDateAndTimeStringWithUnderscore(this DateTime dateTime)
        {
            return 
             $"{dateTime.Microsecond}_{dateTime.Second}_{dateTime.Minute}_{dateTime.Hour}_{dateTime.Day}_{dateTime.Month}_{dateTime.Year}";
        }

    }
}
