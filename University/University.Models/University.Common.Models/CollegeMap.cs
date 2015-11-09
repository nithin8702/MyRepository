using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    public class CollegeMap : CustomField, IModel
    {
        public int CollegeMapId { get; set; }

        public int CollegeId { get; set; }
        public College College { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Department { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Place { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Longitude { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Latitude { get; set; }

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
