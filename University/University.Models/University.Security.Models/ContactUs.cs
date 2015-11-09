using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class ContactUs : CustomField, IModel
    {
        public int ContactUsId { get; set; }

        //public int? ApplicationUserId { get; set; }
        //public ApplicationUser ApplicationUser { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Name { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Email { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string Address { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string Contact { get; set; }
        [StringLength(DataLengthConstant.LENGTH_NOTES)]
        public string Message { get; set; }

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
