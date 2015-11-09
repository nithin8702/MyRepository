
namespace University.Bussiness.Models.ViewModel
{
    public class TrafficNews_vm
    {
        public int TrafficNewsId { get; set; }
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Contact { get; set; }

        public string StreetName { get; set; }
        public string Degree { get; set; }
        public string Description { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }

        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
    }
}
