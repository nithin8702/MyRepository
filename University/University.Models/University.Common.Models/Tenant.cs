using System;
using System.ComponentModel.DataAnnotations;
using University.Common.Models.Enums;

namespace University.Common.Models
{
    public class Tenant : CustomField, IModel
    {
        [Key]
        public int TenantId { get; set; }

        [Timestamp]
        public Byte[] RowVersion { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string TenantName { get; set; }
        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string DisplayName { get; set; }

        public int SessionTime { get; set; }

        public decimal NotificationTime { get; set; }
        
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string HostName { get; set; }
        [StringLength(DataLengthConstant.LENGTH_URL)]
        public string CompanyUrl { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string LogoPath { get; set; }
        

        

        [StringLength(DataLengthConstant.LENGTH_STATUS_CODE)]
        public string StatusCode { get; set; }

        public bool IsRoot { get; set; }

        public int? ParentTenantId { get; set; }

        

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public override string ToString()
        {
            return TenantName;
        }


        public Language Language { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField1 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField2 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField3 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField4 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField5 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField6 { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DESCRIPTION)]
        public string CSSField7 { get; set; }
    }
}
