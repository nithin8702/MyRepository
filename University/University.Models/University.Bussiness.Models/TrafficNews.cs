using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class TrafficNews : CustomField, IModel
    {
        public int TrafficNewsId { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string StreetName { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Degree { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Description { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string ImagePath1 { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string ImagePath2 { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string ImagePath3 { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string ImagePath4 { get; set; }

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
