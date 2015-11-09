using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Utilities;

namespace University.Api.Controllers
{
    public class TestController : UnSecuredController
    {
        public int Get()
        {
            _logger.Info("Test- Called starts");
            UniversityContext context = new UniversityContext();
            string userName = "branchadmin";
            string passWord = userName + "@123";
            string hostName = HttpContext.Current.Request.Url.Host;
            string emailAddress = "admin@abc.com";
            try
            {
                _logger.Info("Test - hostName" + hostName);
                _logger.Info("Test - userName" + userName);
                Tenant tenant = context.Tenants.SingleOrDefault(x => x.HostName == hostName);
                AccountType aType = AccountType.BranchAdmin;
                if (context.ApplicationUsers.Count(x => x.TenantId == tenant.TenantId && x.UserName == userName) == 1)
                {
                    userName = "rootadmin";
                    passWord = userName + "@123";
                    aType = AccountType.RootAdmin;
                }
                string strCurrentDate = DateTime.Now.ToString();
                byte[] passwordSalt = Encryptor.EncryptText(strCurrentDate, userName);
                string se = Convert.ToBase64String(passwordSalt);
                byte[] passwordHash = Encryptor.GenerateHash(passWord, se.ToString());


                if (tenant != null)
                {
                    if (context.ApplicationUsers.Count(x => x.TenantId == tenant.TenantId && x.UserName == userName) == 0)
                    {
                        ApplicationUser admin = new ApplicationUser
                        {
                            UserName = userName,
                            FirstName = userName,
                            LastName = userName,
                            DOB = DateTime.Now,
                            Gender = Gender.Male,
                            EmailAddress = emailAddress,
                            PasswordHash = passwordHash,
                            PasswordSalt = passwordSalt,
                            AccountType = aType,
                            DepartmentId = 1,
                            CollegeId = 1,
                            StatusCode = StatusCodeConstants.ACTIVE,
                            CreatedOn = DateTime.Now,
                            TenantId = tenant.TenantId,
                            Language = Language.English
                        };
                        context.ApplicationUsers.Add(admin);

                        SecurityToken securityToken = new SecurityToken
                        {
                            Token = Guid.NewGuid().ToString("D") + Guid.NewGuid().ToString("D"),
                            TenantId = tenant.TenantId,
                            CreatedBy = admin.ApplicationUserId,
                            CreatedOn = DateTime.Now,
                            IsExpired = false,
                            Expiration = DateTime.Now.AddMinutes(tenant.SessionTime),
                            StatusCode = StatusCodeConstants.ACTIVE,
                            Language = Language.English,
                            ApplicationUserId = admin.ApplicationUserId,
                        };
                        context.SecurityTokens.Add(securityToken);

                        List<ApplicationUserSecurityQuestion> lstUserSecurityAnswer = new List<ApplicationUserSecurityQuestion>();
                        var ques1 = dbContext.SecurityQuestions.SingleOrDefault(x =>
                            x.Question == "What was your childhood nickname?"
                            && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (ques1 != null)
                        {
                            lstUserSecurityAnswer.Add(new ApplicationUserSecurityQuestion
                                {
                                    ApplicationUserId = admin.ApplicationUserId,
                                    SecurityQuestionId = ques1.SecurityQuestionId,
                                    SecurityAnswer = "nickname",
                                    CreatedBy = admin.ApplicationUserId,
                                    CreatedOn = DateTime.Now,
                                    TenantId = tenant.TenantId,
                                    StatusCode = StatusCodeConstants.ACTIVE,
                                    Language = Language.English
                                });
                        }
                        var ques2 = dbContext.SecurityQuestions.SingleOrDefault(x =>
                            x.Question == "What was the name of your first stuffed animal?"
                            && x.TenantId == tenant.TenantId && x.StatusCode == StatusCodeConstants.ACTIVE);
                        if (ques2 != null)
                        {
                            lstUserSecurityAnswer.Add(new ApplicationUserSecurityQuestion
                            {
                                ApplicationUserId = admin.ApplicationUserId,
                                SecurityQuestionId = ques2.SecurityQuestionId,
                                SecurityAnswer = "animal",
                                CreatedBy = admin.ApplicationUserId,
                                CreatedOn = DateTime.Now,
                                TenantId = tenant.TenantId,
                                StatusCode = StatusCodeConstants.ACTIVE,
                                Language = Language.English
                            });
                        }
                        context.ApplicationUserSecurityQuestions.AddRange(lstUserSecurityAnswer);



                        context.SaveChanges();
                    }
                    else
                        _logger.Info("username admin already exists");
                }
                else
                    _logger.Warn("Tenant not satisfied");
            }
            catch (DbEntityValidationException ex1)
            {
                string msg = (ex1.InnerException != null) ? ex1.InnerException.ToString() : ex1.Message;
                _logger.Error("Class Post : " + msg);
            }
            catch (Exception ex2)
            {
                string msg = (ex2.InnerException != null) ? ex2.InnerException.ToString() : ex2.Message;
                _logger.Error("Class Post : " + msg);
            }
            _logger.Info("Test- Called ends");
            return dbContext.ApplicationUsers.Count();
        }
    }
}
