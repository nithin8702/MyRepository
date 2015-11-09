using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using University.Common.Models.Security;

namespace University.Bussiness.Models.ViewModel
{
    public class Notification_vm : UserDetail
    {
        public int NotificationId { get; set; }
        public string Module { get; set; }
        public string Message { get; set; }
        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
        public int Id { get; set; }
        public Nullable<int> ClassDetailId { get; set; }
        public Nullable<int> BookCornerId { get; set; }
        public Nullable<int> TrafficNewsId { get; set; }
        public ClassDetail ClassDetail { get; set; }
        public BookCorner BookCorner { get; set; }
        public TrafficNews TrafficNews { get; set; }
        public string Type { get; set; }
        public string CustomField01 { get; set; }
    }
}
