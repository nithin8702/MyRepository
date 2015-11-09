using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class ComposeMessage : CustomField, IModel
    {
        public int ComposeMessageId { get; set; }

        public int FromUserId { get; set; }
        public ApplicationUser FromUser { get; set; }

        public int ToUserId { get; set; }
        public ApplicationUser ToUser { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Subject { get; set; }
        [StringLength(DataLengthConstant.LENGTH_NOTES)]
        public string Body { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path1 { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path2 { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path3 { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path4 { get; set; }

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
