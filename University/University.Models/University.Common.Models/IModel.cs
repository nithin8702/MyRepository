using System;
using System.ComponentModel.DataAnnotations;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    /// <summary>
    /// Interface for all models(common properties)
    /// </summary>
    public interface IModel
    {
        int? CreatedBy { get; set; }

        DateTime? CreatedOn { get; set; }

        int? LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }

        [Timestamp]
        byte[] RowVersion { get; set; }

        [StringLength(8)]
        string StatusCode { get; set; }

        [Required]
        int TenantId { get; set; }

        Language Language { get; set; }
    }
}
