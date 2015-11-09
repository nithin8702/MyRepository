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
    public class DummyFaculty : CustomField, IModel
    {
        public int DummyFacultyId { get; set; }

        public int ClassDetailId { get; set; }
        public string ClassName { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Location { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Day { get; set; }
        public string Link { get; set; }
        public string ClassRoomNo { get; set; }
        public string BuildingNo { get; set; }
        public string ExamDate { get; set; }
        public string ExamName { get; set; }
        public string Notes { get; set; }

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
