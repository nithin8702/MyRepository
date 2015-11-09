using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class ClassDetail : CustomField, IModel
    {
        public int ClassDetailId { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [StringLength(DataLengthConstant.LENGTH_FREETEXT)]
        public string ClassName { get; set; }

        public int CollegeId { get; set; }
        public College College { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Day { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Location { get; set; }

        [StringLength(DataLengthConstant.LENGTH_ALPHA_NUMERIC_CODE)]
        public string ClassRoomNo { get; set; }

        [StringLength(DataLengthConstant.LENGTH_THOUSAND)]
        public string ExamDate { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string ExamName { get; set; }

        [StringLength(DataLengthConstant.LENGTH_THOUSAND)]
        public string From { get; set; }

        [StringLength(DataLengthConstant.LENGTH_THOUSAND)]
        public string To { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string Link { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string BuildingNo { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NOTES)]
        public string Notes { get; set; }

        public bool IsPasswordProtected { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

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
