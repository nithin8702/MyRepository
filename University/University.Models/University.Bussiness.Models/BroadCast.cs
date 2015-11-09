using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class BroadCast : CustomField, IModel
    {
        public BroadCast()
        {
            //BroadcastMaps = new List<BroadcastMap>();
            RestrictedUsers = new List<ApplicationUser>();
        }
        public int BroadCastId { get; set; }

        public Nullable<int> ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public List<ApplicationUser> RestrictedUsers { get; set; }

        //public Nullable<int> CollegeId { get; set; }
        //public College College { get; set; }

        //public Nullable<int> DepartmentId { get; set; }
        //public Department Department { get; set; }

        public Nullable<int> ClassDetailId { get; set; }
        public ClassDetail ClassDetail { get; set; }

        //public virtual List<BroadcastMap> BroadcastMaps { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string Sub { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NOTES)]
        public string Message { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path_Picture { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path_Doc { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path_Video { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string Path_Voice { get; set; }

        public DateTime? ScheduleDate { get; set; }

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
