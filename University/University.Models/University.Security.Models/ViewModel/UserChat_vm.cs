using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Security.Models.ViewModel
{
    public class UserChat_vm
    {
        public int UserChatId { get; set; }

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

        public string Message { get; set; }
    }
}
