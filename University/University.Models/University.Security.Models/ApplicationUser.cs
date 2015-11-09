using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models
{
    /// <summary>
    /// Application User for Authentication
    /// </summary>
    public class ApplicationUser : CustomField, IModel
    {
        public ApplicationUser()
        {
            DummyColleges = new List<DummyCollege>();
            ProfileVisits = new List<ProfileVisit>();
        }
        [Key]
        public int ApplicationUserId { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string FirstName { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " +  LastName; }
        }


        [StringLength(DataLengthConstant.LENGTH_FREETEXT)]
        public string RefId { get; set; }

        public Gender Gender { get; set; }

        [StringLength(DataLengthConstant.LENGTH_EMAIL)]
        public string EmailAddress { get; set; }

        [StringLength(DataLengthConstant.LENGTH_NAME)]
        public string UserName { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        [StringLength(DataLengthConstant.LENGTH_ALPHA_NUMERIC_CODE)]
        public string Contact { get; set; }

        public int? CollegeId { get; set; }
        public College College { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string ProfilePicturePath { get; set; }

        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string WorkIdPicturePath { get; set; }

        [Column(TypeName = "Date")]
        public DateTime DOB { get; set; }

        public bool IsLocked { get; set; }



        public AccountType AccountType { get; set; }

        public int? loginFailedCount { get; set; }
        public DateTime? lastFailedOn { get; set; }
        public DateTime? lastPasswordChangedOn { get; set; }
        public DateTime? accountLockedOn { get; set; }

        public bool isPasswordExpired { get; set; }

        public Tenant Tenant { get; set; }

        public List<ProfileVisit> ProfileVisits { get; set; }

        public List<DummyCollege> DummyColleges { get; set; }

        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string DeviceType { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string DeviceToken { get; set; }
        [StringLength(DataLengthConstant.LENGTH_DOUBLE_NAME)]
        public string DeviceUDDI { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string PushCertificatePath { get; set; }
        [StringLength(DataLengthConstant.LENGTH_IMAGEPATH)]
        public string DeviceSoundPath { get; set; }
        public string ChannelUri { get; set; }
        public string PushCertificatePassword { get; set; }
        public string OSVersion { get; set; }
        public string BatchingInterval { get; set; }
        public string NavigatePath { get; set; }
        public string RegistrationId { get; set; }

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
