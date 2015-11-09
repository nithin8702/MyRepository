
namespace University.Security.Models.ViewModel
{
    public class Feedback_vm
    {
        public int FeedbackId { get; set; }
        public string LogoPath { get; set; }
        public int ApplicationRating { get; set; }
        public string CustomKey1 { get; set; }
        public int CustomKeyValue1 { get; set; }
        public string CustomKey2 { get; set; }
        public int CustomKeyValue2 { get; set; }
        public string FeedbackStatement { get; set; }
        public string ProblemStatement { get; set; }
        public string SuggestionStatement { get; set; }
        public string PostedDate { get; set; }
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int DaysAgo { get; set; }
    }
}
