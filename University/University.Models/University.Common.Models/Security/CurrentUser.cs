
using University.Common.Models.Enums;
namespace University.Common.Models.Security
{
    public class CurrentUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public AccountType AccountType { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public bool IsLocked { get; set; }
        public override string ToString()
        {
            return string.Format("{0}-{1}", Tenant, UserName);
        }
    }
}
