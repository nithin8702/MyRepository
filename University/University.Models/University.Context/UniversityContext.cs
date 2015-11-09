using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Conventions;
using University.Bussiness.Models;
using University.Common.Models;
using University.Common.Models.Log;
using University.Security.Models;

namespace University.Context
{
    public class UniversityContext : DbContext
    {
        public UniversityContext()
            : base("UniversityConfig")
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer<UniversityContext>(new SeedData());
            Database.SetInitializer<UniversityContext>(null);
        }

        #region DbSet

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<StatusCodeDetail> StatusCodeDetails { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ClassDetail> ClassDetails { get; set; }
        public DbSet<BroadCast> BroadCasts { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        //public DbSet<NeedBook> NeedBooks { get; set; }
        //public DbSet<AvailableBook> AvailableBooks { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        //public DbSet<AdsFrequency> AdsFrequencies { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<ApiDataLog> ApiDataLogs { get; set; }
        public DbSet<ApiErrorLog> ApiErrorLogs { get; set; }
        public DbSet<SecurityToken> SecurityTokens { get; set; }
        public DbSet<BroadCastMessage> BroadCastMessages { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<GoogleMap> GoogleMaps { get; set; }
        public DbSet<TrafficNews> TrafficNews { get; set; }
        public DbSet<ContactBookOwner> ContactBookOwners { get; set; }
        public DbSet<ELearning> ELearnings { get; set; }
        public DbSet<DraftedUser> DraftedUsers { get; set; }
        public DbSet<ComposeMessage> ComposeMessages { get; set; }
        public DbSet<HowToUse> HowToUse { get; set; }
        public DbSet<TenantAddress> TenantAddresses { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<EmailSetting> EmailSettings { get; set; }
        public DbSet<BookCorner> BookCorners { get; set; }
        public DbSet<FavouriteBook> FavouriteBooks { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<ApplicationUserSecurityQuestion> ApplicationUserSecurityQuestions { get; set; }
        public DbSet<CollegeMap> CollegeMaps { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        public DbSet<StudentMark> StudentMarks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AdminMessage> AdminMessages { get; set; }
        public DbSet<AdminMessageUser> AdminMessageUsers { get; set; }
        public DbSet<TenantToken> TenantTokens { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<DummyFaculty> DummyFaculties { get; set; }
        public DbSet<RestrictedUser> RestrictedUsers { get; set; }
        public DbSet<StudentView> StudentViews { get; set; }
        public DbSet<ProfileVisit> ProfileVisits { get; set; }
        public DbSet<DummyCollege> DummyColleges { get; set; }
        public DbSet<TechnicalSupport> TechnicalSupports { get; set; }
        public DbSet<UserChat> UserChats { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.AddBefore<IdKeyDiscoveryConvention>(new DateTime2Convention());
            modelBuilder.Conventions.Add(new DateTime2Convention()); 

            modelBuilder.Entity<Tenant>().MapToStoredProcedures();
            modelBuilder.Entity<StatusCodeDetail>().MapToStoredProcedures();
            modelBuilder.Entity<College>().MapToStoredProcedures();
            modelBuilder.Entity<Department>().MapToStoredProcedures();
            modelBuilder.Entity<ApplicationUser>().MapToStoredProcedures();
            modelBuilder.Entity<ClassDetail>().MapToStoredProcedures();
            modelBuilder.Entity<BroadCast>().MapToStoredProcedures();
            //modelBuilder.Entity<NeedBook>().MapToStoredProcedures();
            //modelBuilder.Entity<AvailableBook>().MapToStoredProcedures();
            modelBuilder.Entity<ContactUs>().MapToStoredProcedures();
            modelBuilder.Entity<AboutUs>().MapToStoredProcedures();
            modelBuilder.Entity<EmailAddress>().MapToStoredProcedures();
            modelBuilder.Entity<Feedback>().MapToStoredProcedures();
            //modelBuilder.Entity<AdsFrequency>().MapToStoredProcedures();
            modelBuilder.Entity<Advertisement>().MapToStoredProcedures();
            modelBuilder.Entity<ApiDataLog>().MapToStoredProcedures();
            modelBuilder.Entity<ApiErrorLog>().MapToStoredProcedures();
            modelBuilder.Entity<SecurityToken>().MapToStoredProcedures();
            modelBuilder.Entity<BroadCastMessage>().MapToStoredProcedures();
            modelBuilder.Entity<StudentSubscription>().MapToStoredProcedures();
            modelBuilder.Entity<GoogleMap>().MapToStoredProcedures();
            modelBuilder.Entity<TrafficNews>().MapToStoredProcedures();
            modelBuilder.Entity<ContactBookOwner>().MapToStoredProcedures();
            modelBuilder.Entity<ELearning>().MapToStoredProcedures();
            modelBuilder.Entity<DraftedUser>().MapToStoredProcedures();
            modelBuilder.Entity<ComposeMessage>().MapToStoredProcedures();
            modelBuilder.Entity<HowToUse>().MapToStoredProcedures();
            modelBuilder.Entity<TenantAddress>().MapToStoredProcedures();
            modelBuilder.Entity<Setting>().MapToStoredProcedures();
            modelBuilder.Entity<EmailSetting>().MapToStoredProcedures();
            modelBuilder.Entity<BookCorner>().MapToStoredProcedures();
            modelBuilder.Entity<FavouriteBook>().MapToStoredProcedures();
            modelBuilder.Entity<SecurityQuestion>().MapToStoredProcedures();
            modelBuilder.Entity<ApplicationUserSecurityQuestion>().MapToStoredProcedures();
            modelBuilder.Entity<CollegeMap>().MapToStoredProcedures();
            modelBuilder.Entity<StudentClass>().MapToStoredProcedures();
            modelBuilder.Entity<StudentAttendance>().MapToStoredProcedures();
            modelBuilder.Entity<StudentMark>().MapToStoredProcedures();
            modelBuilder.Entity<Notification>().MapToStoredProcedures();
            modelBuilder.Entity<AdminMessage>().MapToStoredProcedures();
            modelBuilder.Entity<AdminMessageUser>().MapToStoredProcedures();
            modelBuilder.Entity<TenantToken>().MapToStoredProcedures();
            modelBuilder.Entity<News>().MapToStoredProcedures();
            modelBuilder.Entity<DummyFaculty>().MapToStoredProcedures();
            modelBuilder.Entity<RestrictedUser>().MapToStoredProcedures();
            modelBuilder.Entity<StudentView>().MapToStoredProcedures();
            modelBuilder.Entity<ProfileVisit>().MapToStoredProcedures();
            modelBuilder.Entity<DummyCollege>().MapToStoredProcedures();
            modelBuilder.Entity<TechnicalSupport>().MapToStoredProcedures();
            modelBuilder.Entity<UserChat>().MapToStoredProcedures();
            
            
            //modelBuilder.Conventions.Add(new FunctionsConvention<UniversityContext>("dbo"));

        }
    }
}
