using University.Common.Models.Enums;

namespace University.Bussiness.Models.ViewModel
{
    public class RestrictedUser_vm
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public int? ClassDetailId { get; set; }
        public Module? Module { get; set; }
        public string ModuleName { get; set; }
    }
}
