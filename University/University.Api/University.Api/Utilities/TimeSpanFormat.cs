using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace University.Api.Utilities
{
    public class TimeSpanFormat
    {
        public static string FormatDaysAgo(string date)
        {
            string format = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt = Convert.ToDateTime(date);
                var datespan = DateTimeSpan.CompareDates(DateTime.Now, dt);
                if (datespan.Years > 0 || datespan.Months > 0)
                {
                    format = dt.ToString("dd-MM-yyyy"); //String.Format("{0:dd-MM-yyyy} ", dt.ToShortDateString());
                }
                else
                {
                    if (datespan.Days > 0)
                    {
                        if (datespan.Days == 1)
                        {
                            format = String.Format("{0} Day", datespan.Days);
                        }
                        else
                        {
                            format = String.Format("{0} Days", datespan.Days);
                        }
                    }
                    else if (datespan.Hours > 0)
                    {
                        if (datespan.Hours == 1)
                        {
                            format = String.Format("{0} Hour", datespan.Hours);
                        }
                        else
                        {
                            format = String.Format("{0} Hours", datespan.Hours);
                        }
                    }
                    else
                    {
                        format = String.Format("{0} Minutes", datespan.Minutes);
                    }
                    format += " Ago.";
                }


                //TimeSpan span = (DateTime.Now - Convert.ToDateTime(date));
                //if (span.Days > 0)
                //{
                //    if (span.Days == 1)
                //    {
                //        format = String.Format("{0} Day", span.Days);
                //    }
                //    else
                //    {
                //        format = String.Format("{0} Days", span.Days);
                //    }
                //}
                //else if (span.Hours > 0)
                //{
                //    if (span.Hours == 1)
                //    {
                //        format = String.Format("{0} Hour", span.Hours);
                //    }
                //    else
                //    {
                //        format = String.Format("{0} Hours", span.Hours);
                //    }
                //}
                //else
                //{
                //    format = String.Format("{0} Miutes", span.Minutes);
                //}
            }
            return format;
        }
    }
}