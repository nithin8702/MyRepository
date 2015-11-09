using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Bussiness.Models.ViewModel
{
    public class ComposeMessage_vm
    {
        public List<int> ClassIds { get; set; }
        public List<int> ToUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Contact { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string Path1 { get; set; }
        public string Path2 { get; set; }
        public string Path3 { get; set; }
        public string Path4 { get; set; }
     

    }

    public class ViewMessage_vm
    {
        public int MessageId { get; set; }
        public string Contact { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string Path1 { get; set; }
        public List<string> Path1s { get; set; }
        public string Path2 { get; set; }
        public List<string> Path2s { get; set; }
        public string Path3 { get; set; }
        public List<string> Path3s { get; set; }
        public string Path4 { get; set; }
        public List<string> Path4s { get; set; }

        public int FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string FromFirstName { get; set; }
        public string FromLastName { get; set; }
        public string FromEmailAddress { get; set; }
        public string FromContact { get; set; }

        public int ToUserId { get; set; }
        public string ToUserName { get; set; }
        public string ToFirstName { get; set; }
        public string ToLastName { get; set; }
        public string ToEmailAddress { get; set; }
        public string ToContact { get; set; }
        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
    }
}
