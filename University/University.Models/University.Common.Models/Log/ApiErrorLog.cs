using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models.Enums;

namespace University.Common.Models.Log
{
    public class ApiErrorLog : IModel
    {
        public int ApiErrorLogId { get; set; }
        
        public string ErrorDetail { get; set; }

        public string StackTrace { get; set; }

        public int? ApiDataLogId { get; set; }
        public ApiDataLog ApiDataLog { get; set; }

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
