using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class BroadCastMessage : CustomField, IModel
    {
        public int BroadCastMessageId { get; set; }

        public int? BroadCastId { get; set; }
        public BroadCast BroadCast { get; set; }

        public int? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NOTES)]
        public string Message { get; set; }

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
