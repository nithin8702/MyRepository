using System;
using System.Collections.Generic;
using System.Data.Entity;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Constants;
using University.Security.Models;

namespace University.Context
{
    public class SeedData : DropCreateDatabaseIfModelChanges<UniversityContext>
    {
        protected override void Seed(UniversityContext context)
        {
            try
            {
                Tenant tenant = new Tenant
                {
                    TenantName = "Al-Insaaf",
                    DisplayName = "Al-Insaaf",
                    StatusCode = StatusCodeConstants.ACTIVE,
                    CreatedOn = DateTime.Now,
                    HostName = "www.acku.co",
                    Language = Language.English,
                    SessionTime = 20,
                    NotificationTime = 1,
                    IsRoot = true
                };
                context.Tenants.Add(tenant);
                context.SaveChanges();


                TenantToken tenantToken = new TenantToken
                {
                    Token = Guid.NewGuid().ToString("D") + Guid.NewGuid().ToString("D"),
                    TenantId = tenant.TenantId,
                    Language = Language.English,
                    StatusCode = StatusCodeConstants.ACTIVE,
                };
                context.TenantTokens.Add(tenantToken);

                TenantAddress tenantAddress = new TenantAddress
                {
                    UniversityAddress = "UniversityAddress 1",
                    EmailId = "EmailId",
                    Contact = "Contact",
                    FaxNumber = "FaxNumber",
                    Website = "www",
                    TenantId = tenant.TenantId,
                    CreatedOn = DateTime.Now,
                    Language = Language.English,
                    StatusCode = StatusCodeConstants.ACTIVE
                };
                context.TenantAddresses.Add(tenantAddress);

                AboutUs aboutUS = new AboutUs
                {
                    Description = "Al-Insaaf Company",
                    TenantId = tenant.TenantId,
                    CreatedOn = DateTime.Now,
                    Language = Language.English,
                    StatusCode = StatusCodeConstants.ACTIVE
                };
                context.AboutUs.Add(aboutUS);

                var lstStatusCodeDetail = new List<StatusCodeDetail> 
                {
                    new StatusCodeDetail { TenantId = tenant.TenantId, StatusCodeId = StatusCodeConstants.DRAFT, StatusCodeName="Draft",StatusMessage="Draft", StatusCode = StatusCodeConstants.ACTIVE, CreatedBy = 1, CreatedOn = DateTime.Now },
                    new StatusCodeDetail { TenantId = tenant.TenantId, StatusCodeId = StatusCodeConstants.ACTIVE, StatusCodeName="Active",StatusMessage="Active", StatusCode = StatusCodeConstants.ACTIVE, CreatedBy = 1, CreatedOn = DateTime.Now },
                    new StatusCodeDetail { TenantId = tenant.TenantId, StatusCodeId = StatusCodeConstants.INACTIVE, StatusCodeName="Inactive",StatusMessage="Inactive", StatusCode = StatusCodeConstants.ACTIVE, CreatedBy = 1, CreatedOn = DateTime.Now },
                    new StatusCodeDetail { TenantId = tenant.TenantId, StatusCodeId = StatusCodeConstants.NEW, StatusCodeName="New",StatusMessage="New", StatusCode = StatusCodeConstants.ACTIVE, CreatedBy = 1, CreatedOn = DateTime.Now }

                };
                context.StatusCodeDetails.AddRange(lstStatusCodeDetail);

                //var lstAdsFrequency = new List<AdsFrequency>
                //{
                //    new AdsFrequency{StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,Place=Place.None},
                //    new AdsFrequency{StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,Place=Place.Top},
                //    new AdsFrequency{StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,Place=Place.Center,Frequency=2},
                //    new AdsFrequency{StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,Place=Place.Center,Frequency=3},
                //    new AdsFrequency{StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,Place=Place.Bottom}
                //};
                //context.AdsFrequencies.AddRange(lstAdsFrequency);

                var lstCollege = new List<College>
                {
                    new College{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,CollegeName="College of Law",StatusCode=StatusCodeConstants.ACTIVE,
                        Departments=new List<Department>
                        {
                            new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of Private Law",StatusCode=StatusCodeConstants.ACTIVE,},
                            //new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of Public Law",StatusCode=StatusCodeConstants.ACTIVE,},
                            //new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of Criminal Law"},
                            //new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of International Law",StatusCode=StatusCodeConstants.ACTIVE,}
                        }
                    },
                    //new College{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,CollegeName="College of Arts",StatusCode=StatusCodeConstants.INACTIVE,
                    //Departments=new List<Department>
                    //    {
                    //        new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of Arabic Language",StatusCode = StatusCodeConstants.ACTIVE},
                    //        new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of English Language and Literature",StatusCode=StatusCodeConstants.ACTIVE,},
                    //        new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of History",StatusCode = StatusCodeConstants.ACTIVE,},
                    //        new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of Philosophy",StatusCode = StatusCodeConstants.ACTIVE,},
                    //        new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Department of Information(.media)",StatusCode = StatusCodeConstants.ACTIVE,},
                    //        new Department{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,DepartmentName="Program of the French language and culture",StatusCode=StatusCodeConstants.ACTIVE,},
                    //    }},
                    //new College{CreatedOn = DateTime.Now,TenantId = tenant.TenantId,CollegeName="College of Science",StatusCode=StatusCodeConstants.ACTIVE,
                    //    Departments=new List<Department>
                    //    {
                    //        new Department{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,DepartmentName="Natural and Mathematical Sciences"},
                    //        new Department{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,DepartmentName="Natural and Mathematical Sciences"}
                    //    }
                    //},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Education"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Sharia and Islamic Studies"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Faculty of Medicine"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Faculty of Dentistry"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Faculty of Pharmacy"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Allied Health Sciences"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Engineering and Petroleum"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Business and Administration"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Social Sciences"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Life Sciences"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Faculty of Graduate Studies"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Architecture"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Urban Planning"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Urban Design"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Interior Design"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Project Management Architecture"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Coordination and design of the sites and parks"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="Building Science"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College of Computer Science and Engineering"},
                    //new College{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId,CollegeName="College for Women"},
                };
                context.Colleges.AddRange(lstCollege);

                //var lstHowToUse = new List<HowToUse>
                //{
                //    new HowToUse{Name="Broadcast",Paragraph="Broadcast",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Book Corner",Paragraph="Book Corner",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="College Map",Paragraph="College Map",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Message",Paragraph="Message",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Traffic News",Paragraph="Traffic News",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Profile",Paragraph="Profile",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Schedule",Paragraph="Schedule",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="My Student",Paragraph="My Student",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Attendance",Paragraph="Attendance",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="ELearning",Paragraph="ELearning",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId},
                //    new HowToUse{Name="Subscribe",Paragraph="Subscribe",VideoPath="VideoPath",StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId}
                //};
                //context.HowToUse.AddRange(lstHowToUse);

                var lstSecuritQuestions = new List<SecurityQuestion>
                {
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="In what city did you meet your spouse/significant other?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What was your childhood nickname?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What is the name of your favorite childhood friend?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What street did you live on in third grade?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What is your oldest sibling’s birthday month and year? (e.g., January 1900)"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What is the middle name of your oldest child?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What is your oldest sibling’s middle name?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What school did you attend for sixth grade?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What was your childhood phone number including area code? (e.g., 000-000-0000)"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What was the name of your first stuffed animal?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="What is your maternal grandmother’s maiden name?"},
                    new SecurityQuestion{CreatedOn = DateTime.Now,StatusCode = StatusCodeConstants.ACTIVE,TenantId = tenant.TenantId, Question="In what town was your first job?"},
                };
                context.SecurityQuestions.AddRange(lstSecuritQuestions);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
