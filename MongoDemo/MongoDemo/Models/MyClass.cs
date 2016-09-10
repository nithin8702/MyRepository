using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDemo.Models
{
    public class MyClass
    {
        public object _id { get; set; } //MongoDb uses this field as identity.
        public int n { get; set; }
        public int square { get; set; }
    }
}