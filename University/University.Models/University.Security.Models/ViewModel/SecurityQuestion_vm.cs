using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Security.Models.ViewModel
{
    public class SecurityQuestion_vm
    {
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public List<QuestionDetail> Questions { get; set; }
    }

    public class QuestionDetail
    {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsUserQuestion { get; set; }
    }
}
