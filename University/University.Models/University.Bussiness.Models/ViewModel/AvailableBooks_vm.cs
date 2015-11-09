using University.Common.Models.Enums;

namespace University.Bussiness.Models.ViewModel
{
    public class AvailableBooks_vm
    {
        public int ApplicationUserId { get; set; }
        public int BookId { get; set; }
        public string PostedType { get; set; }
        public string PostedDate { get; set; }
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
        public string ImageUpload1 { get; set; }
        public string ImageUpload2 { get; set; }
        public string ImageUpload3 { get; set; }
        public string ImageUpload4 { get; set; }
    }
}
