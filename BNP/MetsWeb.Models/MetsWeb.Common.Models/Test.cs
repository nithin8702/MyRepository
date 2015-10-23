using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MetsWeb.Common.Models
{
    [DataContract]
    public class Test
    {
        [DataMember]
        public int TestID { get; set; }
        [DataMember]
        public string TestName { get; set; }
    }
}
