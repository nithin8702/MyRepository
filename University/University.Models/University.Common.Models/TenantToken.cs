using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class TenantToken : CustomField
    {
        public int TenantTokenId { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Token { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        [StringLength(DataLengthConstant.LENGTH_STATUS_CODE)]
        public string StatusCode { get; set; }

        [ForeignKey("StatusCode")]
        public virtual StatusCodeDetail StatusCodeDetail { get; set; }

        public Language Language { get; set; }
    }
}
