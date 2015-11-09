using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models.Enums;

namespace University.Common.Models.Log
{
    public class ApiDataLog : IModel
    {
        public int ApiDataLogId { get; set; }

        public string DataLog { get; set; }

        [StringLength(DataLengthConstant.LENGTH_ALPHA_NUMERIC_CODE)]
        public string VerbName { get; set; }

        [StringLength(DataLengthConstant.LENGTH_ALPHA_NUMERIC_CODE)]
        public string ControllerName { get; set; }

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
