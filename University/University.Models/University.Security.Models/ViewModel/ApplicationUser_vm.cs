using System;
using System.Collections.Generic;
using University.Common.Models;
using University.Common.Models.Enums;

namespace University.Security.Models.ViewModel
{
    public class ApplicationUser_vm
    {
        public Tenant Tenant { get; set; }
        public int TenantId { get; set; }
        public int ApplicationUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RefId { get; set; }
        public Gender Gender { get; set; }
        public string GenderName { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Contact { get; set; }
        public DateTime DOB { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountTypeName { get; set; }
        public Language Language { get; set; }
        //public Tenant Tenant { get; set; }
        public Byte[] RowVersion { get; set; }
        public string StatusCode { get; set; }
        public string Token { get; set; }
        public string TenantToken { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ProfilePicturePath { get; set; }
        public string WorkIdPicturePath { get; set; }

        public List<ProfileVisit> ProfileVisits { get; set; }

        public List<DummyCollege> DummyColleges { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string DeviceType { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceUDDI { get; set; }
        public string PushCertificatePath { get; set; }
        public string DeviceSoundPath { get; set; }
        public string ChannelUri { get; set; }
        public string PushCertificatePassword { get; set; }
        public string OSVersion { get; set; }
        public string BatchingInterval { get; set; }
        public string NavigatePath { get; set; }
        public string RegistrationId { get; set; }
    }
}
