using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using University.Common.Models.Enums;
using University.Security.Models.ViewModel;

namespace University.Bussiness.Models.ViewModel
{
    public class FacultyStudents_vm
    {
        public int ApplicationUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefId { get; set; }
        public Gender Gender { get; set; }
        public string GenderName { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Contact { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int ClassDetailId { get; set; }
        public string ClassName { get; set; }
        public string StatusCode { get; set; }
        public List<ApplicationUser_vm> RestrictedUsers { get; set; }
        public bool CanReply { get; set; }
        public string ProfilePicturePath { get; set; }
        public string WorkIdPicturePath { get; set; }
    }
}
