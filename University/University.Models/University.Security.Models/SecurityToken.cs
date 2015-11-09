using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class SecurityToken : CustomField, IModel
    {
        public int SecurityTokenId { get; set; }
        public bool IsExpired { get; set; }
        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Token { get; set; } //Guid.ToString()
        public DateTime Expiration { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        #region IModel

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [Timestamp]
        public Byte[] RowVersion { get; set; }

        public int TenantId { get; set; }

        [StringLength(DataLengthConstant.LENGTH_STATUS_CODE)]
        public string StatusCode { get; set; }

        [ForeignKey("StatusCode")]
        public virtual StatusCodeDetail StatusCodeDetail { get; set; }

        public Language Language { get; set; }

        #endregion
    }
}
