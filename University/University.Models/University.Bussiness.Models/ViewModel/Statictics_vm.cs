using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using University.Security.Models;

namespace University.Bussiness.Models.ViewModel
{
    public class Statictics_vm
    {
        public List<ProfileVisit> ProfileVisits { get; set; }
        public int NumberOfClasses { get; set; }
        public int NumberOfStudents { get; set; }
        public int NumberOfBroadCastsSends { get; set; }
        public int NumberOfBroadCastsReceived { get; set; }
        public int NumberOfMessageSends { get; set; }
        public int NumberOfMessageReceived { get; set; }
        public int NumberOfColleges { get; set; }
        public int NumberOfDepartments { get; set; }
        public int NumberOfMaleFaculty { get; set; }
        public int NumberOfFemaleFaculty { get; set; }
        public int NumberOfMaleStudents { get; set; }
        public int NumberOfFemaleStudents { get; set; }
        public int NumberOfAdvertisements { get; set; }
    }
}
