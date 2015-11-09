using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Common.Models.ViewModel
{
    public class GoogleMap_vm
    {
        public int GoogleMapId { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string CustomField01 { get; set; }
        public string CustomField02 { get; set; }
        public string CustomField03 { get; set; }
        public string CustomField04 { get; set; }
        public string CustomField05 { get; set; }
    }
}
