using System.Collections.Generic;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Common.Models.Security;

namespace University.Security.Models.ViewModel
{
    public class AdminMessage_vm : UserDetail
    {
        public int AdminMessageId { get; set; }
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
        public string PostedDate { get; set; }
        public Department Department { get; set; }
        public string DaysAgo { get; set; }
        public string CustomField01 { get; set; }
        public string CustomField02 { get; set; }
        public string CustomField03 { get; set; }
        public string CustomField04 { get; set; }
    }
}
