using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class ELearning : CustomField, IModel
    {
        public int ELearningId { get; set; }

        public int? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Title { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Link { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Website { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string Desciption { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string ImageUploadPath { get; set; }
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
