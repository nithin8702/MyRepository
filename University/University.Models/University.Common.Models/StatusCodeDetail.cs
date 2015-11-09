using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    /// <summary>
    /// Sattuc Code
    /// </summary>
    public class StatusCodeDetail : IModel
    {

        [Key]
        [StringLength(DataLengthConstant.LENGTH_STATUS_CODE)]
        public string StatusCodeId { get; set; }

        [Timestamp]
        public Byte[] RowVersion { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string StatusCodeName { get; set; }

        public string StatusColourCode { get; set; }

        public string StatusMessage { get; set; }

        [StringLength(DataLengthConstant.LENGTH_STATUS_CODE)]
        public string StatusCode { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }        
        public Language Language { get; set; }        
    }

}
