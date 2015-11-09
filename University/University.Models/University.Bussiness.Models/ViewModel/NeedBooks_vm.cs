using University.Common.Models.Enums;

namespace University.Bussiness.Models.ViewModel
{
    public class NeedBooks_vm
    {
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Contact { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsFavouriteBook { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public string BookLanguage { get; set; }
        public string Description { get; set; }
    }
}
