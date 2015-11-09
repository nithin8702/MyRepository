using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using University.Constants;
using University.Utilities;

namespace University.Api.Extensions
{
    public class DbCommaSeparator
    {
        [DbFunction("MyContext", "DbCommaSeparator")]
        public static List<string> Separate(string value)
        {
            List<string> lstObject = new List<string>();
            string[] strSplitOperator;
            if (!string.IsNullOrEmpty(value))
            {
                strSplitOperator = value.Split(new char[] { TechConstants.Separator }, StringSplitOptions.RemoveEmptyEntries);
                if (strSplitOperator != null && strSplitOperator.Length > 0)
                {
                    foreach (var item in strSplitOperator)
                    {
                        lstObject.Add(item);
                    }
                }
            }
            return lstObject.Trim();
        } 
    }
}