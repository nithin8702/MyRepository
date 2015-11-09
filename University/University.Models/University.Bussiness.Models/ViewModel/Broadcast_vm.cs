using System;
using System.Collections.Generic;
using University.Security.Models;
using University.Security.Models.ViewModel;

namespace University.Bussiness.Models.ViewModel
{
    public class Broadcast_vm
    {
        public int BroadCastId { get; set; }
        //public int BroadcastMapId { get; set; }
        //public int StudentSubscriptionId { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int FacultyId { get; set; }
        public string FacultyUserName { get; set; }
        public string Sub { get; set; }
        public string Message { get; set; }
        public string Path_Picture { get; set; }
        public List<string> Path_Pictures { get; set; }
        public string Path_Doc { get; set; }
        public List<string> Path_Docs { get; set; }
        public string Path_Video { get; set; }
        public List<string> Path_Videos { get; set; }
        public string Path_Voice { get; set; }
        public List<string> Path_Voices { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public List<int> RestrictedUsers { get; set; }
        public bool CanReply { get; set; }
        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
