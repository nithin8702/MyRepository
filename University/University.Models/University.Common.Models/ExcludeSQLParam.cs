using System;

namespace University.Common.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeSQLParamAttribute : Attribute
    {

    }
}
