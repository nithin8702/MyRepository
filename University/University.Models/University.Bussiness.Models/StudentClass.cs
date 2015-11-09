using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Bussiness.Models
{
    public class StudentClass : CustomField, IModel
    {
        public StudentClass()
        {
            StudentAttendances = new List<StudentAttendance>();
            StudentMarks = new List<StudentMark>();
        }
        public int StudentClassId { get; set; }        

        public Nullable<int> ClassDetailId { get; set; }
        public ClassDetail ClassDetail { get; set; }

        public List<StudentAttendance> StudentAttendances { get; set; }

        public List<StudentMark> StudentMarks { get; set; }        

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
