using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Common.Models.Enums;
using University.Utilities;

namespace University.Api.Utilities
{
    public static class EnumUtilities
    {
        public static List<Day> ToDayEnums(this List<string> value)
        {
            List<Day> lstDay = new List<Day>();
            if (value.HasValue())
            {
                foreach (var item in value)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        Day day = Day.Monday;
                        switch (item.ToLower().Trim())
                        {
                            case "sunday":
                                day = Day.Sunday;
                                break;
                            case "monday":
                                day = Day.Monday;
                                break;
                            case "tuesday":
                                day = Day.Tuesday;
                                break;
                            case "wednesday":
                                day = Day.Wednesday;
                                break;
                            case "thursday":
                                day = Day.Thursday;
                                break;
                            case "friday":
                                day = Day.Friday;
                                break;
                            case "saturday":
                                day = Day.Saturday;
                                break;
                        }
                        lstDay.Add(day);
                    }
                }
            }
            return lstDay;
        }

    }
}