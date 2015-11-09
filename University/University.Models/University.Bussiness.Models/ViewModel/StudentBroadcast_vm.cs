using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Bussiness.Models.ViewModel
{
    public class StudentBroadcast_vm
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int BroadCastId { get; set; }
        public string Password { get; set; }
        public int BroadcastMapId { get; set; }
        public List<Broadcast_vm> Broadcasts_vm { get; set; }
    }
}
