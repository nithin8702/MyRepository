using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using University.Bussiness.Models;
using University.Bussiness.Models.ViewModel;
using University.Common.Models;
using University.Common.Models.Enums;
using University.Constants;
using University.Context;
using University.Security.Models;
using University.Utilities;


namespace ConsoleApplicationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //bool test=(lstBroadCast.Count(x=>x.ClassId==1)==0?true:false);
                UniversityContext dbcontext = new UniversityContext();
                var sT1 = dbcontext.SecurityTokens.SingleOrDefault(x => x.Token == "b51524c2-3869-4696-a670-51b1b9c3d523fe2d1b2d-4e74-4907-b33a-d172901683b1");
                var sT2 = dbcontext.SecurityTokens.Include("ApplicationUser").SingleOrDefault(x => x.Token == "b51524c2-3869-4696-a670-51b1b9c3d523fe2d1b2d-4e74-4907-b33a-d172901683b1");




                var lstCB = dbcontext.ContactBookOwners.Where(x => x.TenantId == 5).ToList();
                if (lstCB.HasValue())
                {
                    foreach (var item in lstCB)
                    {
                        item.CustomField06 = StatusCodeConstants.DELETED;
                    }
                }
                dbcontext.SaveChanges();
                var data3 = dbcontext.ApplicationUsers;


                //CreateAdminMessage(dbcontext);


                //CreateStudentAttendance(dbcontext);

                var data1 = dbcontext.StudentClasses.Include("StudentAttendances").ToList();
                var data2 = dbcontext.StudentAttendances.ToList();




                var data = dbcontext.Database.SqlQuery<BookCorner>("select * from bookcorners where bookcornerid={0}", new object[] { 1 }).ToList();

                int studentId = 4;
                var map = dbcontext.StudentSubscriptions
                        .Where(x => x.ApplicationUserId == studentId && x.TenantId == 1
                            && x.StatusCode == StatusCodeConstants.ACTIVE).ToList();
                var b = (from sub in dbcontext.StudentSubscriptions.Where(x => x.ApplicationUserId == studentId && x.TenantId == 1
                         && x.StatusCode == StatusCodeConstants.ACTIVE)
                         join
                             bC in dbcontext.BroadCasts.Include("ApplicationUser") on sub.ClassDetailId equals bC.ClassDetailId
                         join
                         user in dbcontext.ApplicationUsers on bC.ApplicationUserId equals user.ApplicationUserId
                         select new Broadcast_vm
                         {
                             CanReply = ((bC.RestrictedUsers.Count(x => x.ApplicationUserId == studentId) == 0) ? true : false),
                             BroadCastId = bC.BroadCastId,
                             ClassId = bC.ClassDetailId.Value,
                             ClassName = bC.ClassDetail.ClassName,
                             DepartmentId = bC.ClassDetail.DepartmentId,
                             DepartmentName = bC.ClassDetail.Department.DepartmentName,
                             FacultyId = bC.ApplicationUserId.Value,
                             FacultyUserName = user.FirstName,
                             CollegeId = bC.ClassDetail.CollegeId,
                             CollegeName = bC.ClassDetail.College.CollegeName,
                             Sub = bC.Sub,
                             Message = bC.Message,
                             Path_Picture = bC.Path_Picture,
                             Path_Doc = bC.Path_Doc,
                             Path_Video = bC.Path_Video,
                             Path_Voice = bC.Path_Voice
                         }).ToList();
                var a1 = dbcontext.BroadCasts.Include("RestrictedUsers").ToList();
                //CreateUser(dbcontext, AccountType.Admin);
                //CreateUser(dbcontext, AccountType.Faculty, "user1", Gender.Male);
                //CreateUser(dbcontext, AccountType.Faculty, "user2", Gender.Male);
                //CreateUser(dbcontext, AccountType.Faculty, "user3", Gender.Female);
                //CreateUser(dbcontext, AccountType.Faculty, "user4", Gender.Female);
                //CreateUser(dbcontext, AccountType.Student, "user5", Gender.Male);
                //CreateUser(dbcontext, AccountType.Student, "user6", Gender.Male);
                //CreateUser(dbcontext, AccountType.Student, "user7", Gender.Female);
                //CreateUser(dbcontext, AccountType.Student, "user8", Gender.Female);
                //CreateClassDetail(dbcontext);
                //Console.ReadLine();


                //SecurityToken securityToken = context.SecurityTokens.SingleOrDefault(x => x.StatusCode == StatusCodeConstants.ACTIVE);
                //securityToken.Expiration = DateTime.Now.AddMinutes(30);
                //context.SaveChanges();


                //var bd = context.BroadCasts.Include("BroadcastMaps").ToList();
                //var data1 = context.StatusCodeDetails.ToList();
                //var data2 = context.AdsFrequencies.FirstOrDefault();
                //if (data2 != null)
                //{
                //    Console.WriteLine(data2.Place);
                //    Console.WriteLine(data2.Place.GetType());
                //}

                //ValidateUser(context);
                //CreateBroadCast(context);
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }

        }

        private static void CreateAdminMessage(UniversityContext dbcontext)
        {
            try
            {
                MessageType ty = MessageType.OnlyMaleFaculty;
                AdminMessage msg = new AdminMessage { Message = "Hello 1", MessageType = ty };
                dbcontext.AdminMessages.Add(msg);

                if (ty == MessageType.OnlyMaleFaculty)
                {
                    var data = dbcontext.ApplicationUsers.Where(x => x.Gender == Gender.Male && x.AccountType == AccountType.Faculty).Select(y => y.ApplicationUserId).ToList();
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            AdminMessageUser use = new AdminMessageUser
                            {
                                AdminMessageId = msg.AdminMessageId,
                                ApplicationUserId = item
                            };
                            dbcontext.AdminMessageUsers.Add(use);
                        }
                    }
                }
                else if (ty == MessageType.OnlyMaleFaculty)
                {

                }
                dbcontext.SaveChanges();
                var da = dbcontext.AdminMessages.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void CreateStudentAttendance(UniversityContext dbcontext)
        {
            StudentClass att = new StudentClass
            {
                ClassDetailId = 1,
                //StudentId = 2,
                TenantId = 1,
                StatusCode = StatusCodeConstants.ACTIVE
            };

            List<StudentAttendance> lstStudentAttendanceDetail = new List<StudentAttendance>();
            lstStudentAttendanceDetail.Add(new StudentAttendance { StudentAttendanceId = att.StudentClassId, ClassDate = DateTime.Now, IsAbsent = true });
            lstStudentAttendanceDetail.Add(new StudentAttendance { StudentAttendanceId = att.StudentClassId, ClassDate = DateTime.Now.AddDays(1), IsAbsent = true });


            List<StudentMark> lstStudentMark = new List<StudentMark>();
            lstStudentMark.Add(new StudentMark { ExamDate = DateTime.Now, ExamName = "e1", Mark = 67 });
            lstStudentMark.Add(new StudentMark { ExamDate = DateTime.Now.AddDays(1), ExamName = "e2", Mark = 68 });

            dbcontext.StudentClasses.Add(att);
            dbcontext.StudentAttendances.AddRange(lstStudentAttendanceDetail);
            dbcontext.StudentMarks.AddRange(lstStudentMark);
            dbcontext.SaveChanges();
        }

        //private static DataSet GetAcfOutgoingData(CurrentUser currentUser)
        //{
        //    string connectionString = StoredProcedureHelper.GetConnectionString(currentUser);
        //    SqlParameter[] sqlParam = new SqlParameter[1];
        //    sqlParam[0] = new SqlParameter { ParameterName = "@tenantId", DbType = DbType.Int32, IsNullable = false, SqlValue = currentUser.TenantId };
        //    var data = StoredProcedureHelper.GetDataSet(connectionString, ref sqlParam, "GetAcfOutgoings");
        //    return data;
        //}

        private static void CreateClassDetail(UniversityContext dbcontext)
        {
            try
            {
                Day d1 = Day.Monday;
                Day d2 = Day.Wednesday;
                List<Day> lst = new List<Day>();
                lst.Add(d1);
                lst.Add(d2);
                ClassDetail cD = new ClassDetail
                {
                    ClassName = "C#",
                    TenantId = 1,
                    StatusCode = "A",
                    CollegeId = 1,
                    DepartmentId = 1,
                    ApplicationUserId = 1,
                    //Days = lst
                };
                dbcontext.ClassDetails.Add(cD);
                dbcontext.SaveChanges();

                var c2 = dbcontext.ClassDetails.SingleOrDefault(x => x.ClassDetailId == 2);
                if (c2 != null)
                {
                    Day d = Day.Friday;
                    //c2.Days.Add(d);
                    dbcontext.SaveChanges();
                }

                var data = dbcontext.ClassDetails.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private static void ValidateUser(UniversityContext context)
        {
            string userName = "nithin";
            string passWord = "nithin@123";
            try
            {
                var user = context.ApplicationUsers.SingleOrDefault(x => x.UserName == userName);
                if (user != null)
                {
                    byte[] strSalt = user.PasswordSalt;
                    string salt = Convert.ToBase64String(strSalt);
                    byte[] dbPasswordHash = user.PasswordHash;
                    byte[] userPasswordHash = Encryptor.GenerateHash(passWord, salt);
                    bool chkPassword = Encryptor.CompareByteArray(dbPasswordHash, userPasswordHash);
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        private static void CreateBroadCast(UniversityContext context)
        {
            try
            {
                ClassDetail cd = new ClassDetail
                {
                    ApplicationUserId = 1,
                    CollegeId = 1,
                    DepartmentId = 1,
                    TenantId = 1
                };
                context.ClassDetails.Add(cd);
                context.SaveChanges();
                BroadCast bC = new BroadCast
                {
                    ApplicationUserId = 1,
                    ClassDetailId = 1,
                    //CollegeId = 1,
                    CreatedBy = 1,
                    CreatedOn = DateTime.Now
                };
                context.BroadCasts.Add(bC);
                context.SaveChanges();


                var obj = context.BroadCasts.SingleOrDefault(x => x.BroadCastId == bC.BroadCastId);
                //obj.BroadcastMaps.Add(new BroadcastMap
                //{
                //    ApplicationUserId = 1,
                //    Language = Language.English
                //});
                context.SaveChanges();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static void CreateUser(UniversityContext context, AccountType atype)
        {
            string userName = atype.ToString();
            string passWord = "admin@123";
            string hostName = "www.al-insaaf.com";
            string emailAddress = "admin@abc.com";
            try
            {
                string strCurrentDate = DateTime.Now.ToString();
                byte[] passwordSalt = Encryptor.EncryptText(strCurrentDate, userName);
                string se = Convert.ToBase64String(passwordSalt);
                byte[] passwordHash = Encryptor.GenerateHash(passWord, se.ToString());
                Tenant tenant = context.Tenants.SingleOrDefault(x => x.HostName == hostName);
                if (tenant != null)
                {
                    ApplicationUser admin = new ApplicationUser
                    {
                        UserName = userName,
                        FirstName = "Admin",
                        LastName = "Admin",
                        Gender = Gender.Male,
                        EmailAddress = emailAddress,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        AccountType = atype,
                        StatusCode = StatusCodeConstants.ACTIVE,
                        CreatedOn = DateTime.Now,
                        TenantId = tenant.TenantId,
                        Language = Language.English
                    };
                    context.ApplicationUsers.Add(admin);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
            catch (Exception ex2)
            {
                throw ex2;

            }
        }

        private static void CreateUser(UniversityContext context, AccountType atype, string userName, Gender gender)
        {
            //string userName = userName;
            string passWord = "admin@123";
            string hostName = "www.al-insaaf.com";
            string emailAddress = "admin@abc.com";
            try
            {
                string strCurrentDate = DateTime.Now.ToString();
                byte[] passwordSalt = Encryptor.EncryptText(strCurrentDate, userName);
                string se = Convert.ToBase64String(passwordSalt);
                byte[] passwordHash = Encryptor.GenerateHash(passWord, se.ToString());
                Tenant tenant = context.Tenants.SingleOrDefault(x => x.HostName == hostName);
                if (tenant != null)
                {
                    ApplicationUser admin = new ApplicationUser
                    {
                        UserName = userName,
                        FirstName = "Admin",
                        LastName = "Admin",
                        Gender = gender,
                        EmailAddress = emailAddress,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        AccountType = atype,
                        StatusCode = StatusCodeConstants.ACTIVE,
                        CreatedOn = DateTime.Now,
                        TenantId = tenant.TenantId,
                        Language = Language.English
                    };
                    context.ApplicationUsers.Add(admin);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
            catch (Exception ex2)
            {
                throw ex2;

            }
        }
    }
}

//public static void Customize()
//        {
//            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//        }


//public static HttpResponseMessage ReturnContent(object returnObj, IContentNegotiator content, MediaTypeFormatterCollection formatter, HttpRequestMessage request)
//        {
//            IContentNegotiator negotiator = content;
//            ContentNegotiationResult result = null;
//            result = negotiator.Negotiate(typeof(object), request, formatter);
//            return new HttpResponseMessage()
//            {
//                StatusCode = HttpStatusCode.OK,
//                Content = new ObjectContent<object>(returnObj, result.Formatter, result.MediaType.MediaType)
//            };
//        }

//return Content.ReturnContent(returnObj, this.Configuration.Services.GetContentNegotiator(), this.Configuration.Formatters, this.Request);


//DateTime datae = DateTime.Now;
//string a = datae.Year.ToString();
//string b1 = a.Substring(a.Length - 1, 1);
//string c = a.LastIndexOf(a).ToString().Replace(b1, "_");

//string year = a.Replace(a.Last().ToString(), "_"); ;

//string d = datae.Month.ToString("d2");
//string month = d.Replace(d.Last().ToString(),"_");

//string e = datae.Day.ToString("d2");
//string day = e.Replace(e.Last().ToString(), "_");