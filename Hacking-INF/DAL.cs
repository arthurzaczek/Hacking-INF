using Autofac;
using Hacking_INF.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace Hacking_INF
{
    public class EntityFrameworkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<HackingInfContext>()
                .As<IDAL>()
                .InstancePerLifetimeScope();

            // Setup initializer
            Database.SetInitializer<HackingInfContext>(
                new MigrateDatabaseToLatestVersion<HackingInfContext, Migrations.Configuration>());

            // Upgrade database NOW!
            using (var ctx = new HackingInfContext())
            {
                ctx.Database.Initialize(false);
            }
        }
    }
    public interface IDAL
    {
        IQueryable<User> Users { get; }
        IQueryable<ExampleResult> ExampleResults { get; }
        IQueryable<ReportedCompilerMessages> ReportedCompilerMessages { get; }

        User CreateUser();
        ExampleResult CreateExampleResult();
        void CleanupOldReportedCompilerMessages();
        ReportedCompilerMessages CreateReportedCompilerMessages();

        void SaveChanges();
        IEnumerable<object> GetModifiedEntities();
    }

    public class HackingInfContext : DbContext, IDAL
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(HackingInfContext));

        public HackingInfContext()
            : base("HackingInfEntities")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ExampleResult> ExampleResults { get; set; }
        public DbSet<ReportedCompilerMessages> ReportedCompilerMessages { get; set; }

        public User CreateUser()
        {
            var obj = new User();
            this.Users.Add(obj);
            return obj;
        }

        public ExampleResult CreateExampleResult()
        {
            var obj = new ExampleResult();
            this.ExampleResults.Add(obj);
            return obj;
        }

        public ReportedCompilerMessages CreateReportedCompilerMessages()
        {
            var obj = new ReportedCompilerMessages();
            this.ReportedCompilerMessages.Add(obj);
            return obj;
        }

        public void CleanupOldReportedCompilerMessages()
        {
            var toDelete = DateTime.Now.AddMonths(-3);
            foreach(var obj in this.ReportedCompilerMessages.Where(i => i.Date < toDelete).ToList())
            {
                this.ReportedCompilerMessages.Remove(obj);
            }
        }

        public new void SaveChanges()
        {
            try
            {
                base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var d in ex.EntityValidationErrors)
                {
                    _log.ErrorFormat("    {0}", d.Entry.Entity.GetType().Name);
                    foreach (var msg in d.ValidationErrors)
                    {
                        _log.ErrorFormat("        {0}: {1}", msg.PropertyName, msg.ErrorMessage);
                    }
                }
                throw;
            }
        }

        IQueryable<User> IDAL.Users
        {
            get { return this.Users; }
        }

        IQueryable<ExampleResult> IDAL.ExampleResults
        {
            get { return this.ExampleResults.Include(i => i.User); }
        }

        IQueryable<ReportedCompilerMessages> IDAL.ReportedCompilerMessages
        {
            get { return this.ReportedCompilerMessages.Include(i => i.User); }
        }

        public IEnumerable<object> GetModifiedEntities()
        {
            return this.ChangeTracker
                .Entries()
                .Where(i => i.State == EntityState.Modified)
                .Select(i => i.Entity);
        }
    }
}