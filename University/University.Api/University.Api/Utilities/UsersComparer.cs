using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using University.Security.Models;

namespace University.Api.Utilities
{
    public class UsersComparer : IEqualityComparer<ApplicationUser>
    {
        public bool Equals(ApplicationUser x, ApplicationUser y)
        {
            if (x.UserName == y.UserName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(ApplicationUser obj)
        {
            return obj.UserName.GetHashCode();
        }
    }
}