using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MetsWeb.Loan.Models
{
    [DataContract]
    public class Loan
    {
        [DataMember]
        public int LoanID { get; set; }
        [DataMember]
        public string Comments { get; set; }
    }
}
