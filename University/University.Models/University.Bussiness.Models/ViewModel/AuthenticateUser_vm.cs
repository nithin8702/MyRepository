using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Bussiness.Models.ViewModel
{
    public class AuthenticateUser_vm
    {
        //public string StatusCode { get; set; }
        public int UserId { get; set; }
        public string AuthenticateToken { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string WorkIdPicturePath { get; set; }
        public string ProfilePicturePath { get; set; }
        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
        public string Contact { get; set; }
    }
}
