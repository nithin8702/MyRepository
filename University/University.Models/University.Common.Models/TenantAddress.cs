using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    public class TenantAddress : CustomField, IModel
    {
        public int TenantAddressId { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string UniversityAddress { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string EmailId { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string Contact { get; set; }
        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string FaxNumber { get; set; }
        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Website { get; set; }

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
