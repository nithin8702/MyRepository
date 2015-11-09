using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Security.Models;

namespace University.Bussiness.Models
{
    public class BroadcastMap
    {
        //public BroadcastMap()
        //{
        //    BroadCasts = new List<BroadCast>();
        //}
        [Key]
        public int BroadcastMapId { get; set; }

        public int? BroadCastId { get; set; }
        public BroadCast BroadCast { get; set; }
        
        [Column("StudentId")]
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        //public Nullable<int> ClassDetailId { get; set; }
        //public ClassDetail ClassDetail { get; set; }

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
