using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Bussiness.Models.ViewModel
{
    public class ELearning_vm
    {
        public int ELearningId { get; set; }
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Contact { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Website { get; set; }
        public string Desciption { get; set; }
        public string ImageUploadPath { get; set; }
        public string PostedDate { get; set; }
        public string DaysAgo { get; set; }
    }
}
