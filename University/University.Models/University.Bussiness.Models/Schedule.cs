using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Bussiness.Models
{
    public class Schedule : IModel
    {
        public int ScheduleId { get; set; }
        public int ClassDetailId { get; set; }
        public ClassDetail ClassDetail { get; set; }
        public Nullable<int> CollegeId { get; set; }
        public College College { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Department Department { get; set; }
        //[StringLength(DataLengthConstant.LENGTH_CODE)]
        //public string Section { get; set; }
        public int Days { get; set; }
        public DateTime ClassTime { get; set; }
        [StringLength(DataLengthConstant.LENGTH_FREETEXT)]
        public string Location { get; set; }
        [StringLength(DataLengthConstant.LENGTH_CODE)]
        public string BuildingNo { get; set; }
        [StringLength(DataLengthConstant.LENGTH_CODE)]
        public string ClassRoomNo { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string FacultyName { get; set; }
        //public int  Credits { get; set; }
        public DateTime ExamDate { get; set; }
        //public int Absent { get; set; }
        //[StringLength(DataLengthConstant.LENGTH_NOTES)]
        //public string Notes { get; set; }
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
