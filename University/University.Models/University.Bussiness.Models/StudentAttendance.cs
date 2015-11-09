﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class StudentAttendance : CustomField, IModel
    {
        public int StudentAttendanceId { get; set; }

        public int StudentClassId { get; set; }
        public StudentClass StudentClass { get; set; }

        [Column("StudentId")]
        public int? StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public bool IsPresent { get; set; }

        public bool IsAbsent { get; set; }

        public bool IsLate { get; set; }

        public DateTime? LateTime { get; set; }

        public DateTime ClassDate { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string Comments { get; set; }

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
