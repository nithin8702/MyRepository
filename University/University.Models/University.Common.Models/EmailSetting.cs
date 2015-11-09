using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    public class EmailSetting : CustomField, IModel
    {
        public int EmailSettingId { get; set; }
        public string Port { get; set; }
        public bool EnableSSL { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }

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
