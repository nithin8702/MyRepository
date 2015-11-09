using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class StudentAttendanceDetail:IModel
    {
        public int StudentAttendanceDetailId { get; set; }

        public int StudentClassId { get; set; }
        public StudentClass StudentClass { get; set; }

        [Column("StudentId")]
        public int? StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public decimal NoOfDaysPresent { get; set; }
        public decimal NoOfDaysAbsent { get; set; }
        public decimal NoOfDaysLate { get; set; }

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
