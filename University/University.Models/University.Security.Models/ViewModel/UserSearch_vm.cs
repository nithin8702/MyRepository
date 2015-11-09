using System.Collections.Generic;
using University.Common.Models.Enums;

namespace University.Security.Models.ViewModel
{
    public class UserSearch_vm
    {
        public AccountType AccountType { get; set; }
        public MessageType MessageType { get; set; }
        public List<int> ClassIds { get; set; }
        public string Name { get; set; }
        public Module Module { get; set; }
        public bool IsExcludeStudent { get; set; }
    }
}
