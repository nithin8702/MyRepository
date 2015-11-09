﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    public class DummyCollege : CustomField, IModel
    {
        public int DummyCollegeId { get; set; }


        public int? ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string CollegeName { get; set; }
        public string DepartmentName { get; set; }
        public string Timing { get; set; }
        public string Location { get; set; }
        public string ClassRoomNo { get; set; }
        public string Link { get; set; }
        public string BuildingNo { get; set; }
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
