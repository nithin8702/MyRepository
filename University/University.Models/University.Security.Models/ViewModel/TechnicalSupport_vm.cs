using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Security.Models.ViewModel
{
    public class TechnicalSupport_vm
    {
        public int TechnicalSupportId { get; set; }
        public int ApplicationUserId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Details { get; set; }

        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNo { get; set; }
        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
    }
}
