using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    public class HowToUse : CustomField, IModel
    {
        public int HowToUseId { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Name { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NOTES)]
        public string Paragraph { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string VideoPath { get; set; }

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
