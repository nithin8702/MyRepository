using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Bussiness.Models.ViewModel
{
    public class ContactBookOwner_vm
    {
        public int ApplicationUserId { get; set; }
        public int ContactBookOwnerId { get; set; }
        public int ToUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Contact { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string PostedDate { get; set; }
        public string CustomField01 { get; set; }
        public string DaysAgo { get; set; }
        public string BookId { get; set; }
        public string Status { get; set; }
        public string BookName { get; set; }
    }
}
