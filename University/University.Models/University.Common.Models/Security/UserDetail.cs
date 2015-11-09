using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Common.Models.Security
{
    public abstract class UserDetail
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }
}
