using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;

namespace GIBS.Module.ReportViewer.Repository
{
    public class ReportViewerContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Models.ReportViewer> ReportViewer { get; set; }

        public ReportViewerContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.ReportViewer>().ToTable(ActiveDatabase.RewriteName("GIBSReportViewer"));
        }
    }
}
