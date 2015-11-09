using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class Advertisement : CustomField, IModel
    {
        public int AdvertisementId { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string Screen { get; set; }

        public Place Place { get; set; }
        public Frequency Frequency { get; set; }
        public Mode Mode { get; set; }

        [StringLength(DataLengthConstant.LENGTH_THOUSAND)]
        public string ImagePath { get; set; }

        //public int AdsFrequencyId { get; set; }
        //public AdsFrequency AdsFrequency { get; set; }

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
