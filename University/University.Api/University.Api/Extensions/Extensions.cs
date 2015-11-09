using System;
using University.Common.Models;
using University.Common.Models.Security;
using University.Security.Models.ViewModel;
using System.Linq;
using System.Text;

namespace University.Api.Extensions
{
    public static class Extensions
    {
        public static bool HasValue(this Tenant tenant)
        {
            return (tenant != null && tenant.TenantId > 0);
        }

        public static bool HasValue(this CurrentUser currentUser)
        {
            return (currentUser != null && currentUser.TenantId > 0 && currentUser.UserId > 0);
        }        

        public static bool HasValue(this ApiViewModel apiViewModel)
        {
            bool isValid = false;
            if(apiViewModel != null && apiViewModel.custom!=null)
            {
                isValid = true;
            }
            return isValid;
        }

        public static Tuple<string,string,string> FormHiddenDate(this DateTime value)
        {
            DateTime result;
            string yearTmp = string.Empty;
            string monthTmp = string.Empty;
            string dayTmp = string.Empty;
            string year = string.Empty;
            string month = string.Empty;
            string day = string.Empty;
            if(DateTime.TryParse(value.ToString(),out result ))
            {
                yearTmp = value.Year.ToString();
                monthTmp = value.Month.ToString("d2");
                dayTmp = value.Day.ToString("d2");

                year = yearTmp.Replace(yearTmp.Last().ToString(), "_");
                month = monthTmp.Replace(monthTmp.Last().ToString(), "_");
                day = dayTmp.Replace(dayTmp.Last().ToString(), "_");
            }
            return Tuple.Create<string, string, string>(year, month, day);
        }

        public static string RandomString(this int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}