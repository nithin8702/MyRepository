using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class Setting : CustomField, IModel
    {
        public int SettingId { get; set; }

        public int? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsNotify { get; set; }
        public bool IsBookCorner { get; set; }
        public bool IsBroadcast { get; set; }
        public bool IsTrafficNews { get; set; }
        public bool IsMessage { get; set; }
        public bool IsChat { get; set; }

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
