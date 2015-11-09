using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace University.Common.Models.ViewModel
{
    public class Tenant_vm
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string DisplayName { get; set; }
        public int SessionTime { get; set; }
        public decimal NotificationTime { get; set; }
        public string CompanyUrl { get; set; }
        public string LogoPath { get; set; }
        public bool IsRoot { get; set; }
        public int? ParentTenantId { get; set; }
        public string Token { get; set; }
    }
}
