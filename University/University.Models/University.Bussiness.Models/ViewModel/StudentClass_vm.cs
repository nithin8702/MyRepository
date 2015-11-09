using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using University.Common.Models.Security;

namespace University.Bussiness.Models.ViewModel
{
    public class StudentClass_vm
    {
        public int StudentClassId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        //public string UserName { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string EmailAddress { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IEnumerable<StudentAttendance_vm> StudentAttendance_vm { get; set; }
        public IEnumerable<StudentMark_vm> StudentMark_vm { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Grade { get; set; }
    }

    public class StudentAttendanceDetail_vm
    {
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public int NoOfDaysPresent { get; set; }
        public int NoOfDaysAbsent { get; set; }
        public int NoOfDaysLate { get; set; }
    }

    public class StudentAttendance_vm:UserDetail
    {
        public int StudentAttendanceId { get; set; }
        public int StudentId { get; set; }
        public string RefId { get; set; }
        public bool IsPresent { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsLate { get; set; }
        public DateTime? LateTime { get; set; }
        public DateTime ClassDate { get; set; }
        public string Comments { get; set; }

        //For student view mark
        public int classId { get; set; }
        public int? userId { get; set; }
    }

    public class StudentMark_vm
    {
        public int StudentMarkId { get; set; }
        public int StudentId { get; set; }
        public string RefId { get; set; }
        public string StudentName { get; set; }
        public string ExamName { get; set; }
        public DateTime ExamDate { get; set; }
        public decimal Mark { get; set; }
        public string Comments { get; set; }

        //For student view mark
        public int classId { get; set; }
        public int? userId { get; set; }
    }
}
