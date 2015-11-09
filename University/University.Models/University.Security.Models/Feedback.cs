using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class Feedback : CustomField, IModel
    {
        public int FeedbackId { get; set; }

        public int? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
                
        public int ApplicationRating { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CustomKey1 { get; set; }

        public int CustomKeyValue1 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CustomKey2 { get; set; }
        
        public int CustomKeyValue2 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string FeedbackStatement { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string ProblemStatement { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string SuggestionStatement { get; set; }

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
