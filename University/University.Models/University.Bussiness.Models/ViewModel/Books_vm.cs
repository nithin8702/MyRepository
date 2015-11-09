using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Bussiness.Models.ViewModel
{
    public class Books_vm
    {
        public int BookId { get; set; }
        public int CollegeId { get; set; }
        public int DepartmentId { get; set; }
        public string BookName { get; set; }
        public int BookLanguageId { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public string path_img1 { get; set; }
        public string path_img2 { get; set; }
        public string path_img3 { get; set; }
        public string path_img4 { get; set; }
    }
}
