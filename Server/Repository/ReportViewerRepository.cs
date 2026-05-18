using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;

namespace GIBS.Module.ReportViewer.Repository
{
    public interface IReportViewerRepository
    {
        IEnumerable<Models.ReportViewer> GetReportViewers(int ModuleId);
        Models.ReportViewer GetReportViewer(int ReportViewerId);
        Models.ReportViewer GetReportViewer(int ReportViewerId, bool tracking);
        Models.ReportViewer AddReportViewer(Models.ReportViewer ReportViewer);
        Models.ReportViewer UpdateReportViewer(Models.ReportViewer ReportViewer);
        void DeleteReportViewer(int ReportViewerId);
    }

    public class ReportViewerRepository : IReportViewerRepository, ITransientService
    {
        private readonly IDbContextFactory<ReportViewerContext> _factory;

        public ReportViewerRepository(IDbContextFactory<ReportViewerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.ReportViewer> GetReportViewers(int ModuleId)
        {
            using var db = _factory.CreateDbContext();
            return db.ReportViewer.Where(item => item.ModuleId == ModuleId).ToList();
        }

        public Models.ReportViewer GetReportViewer(int ReportViewerId)
        {
            return GetReportViewer(ReportViewerId, true);
        }

        public Models.ReportViewer GetReportViewer(int ReportViewerId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return db.ReportViewer.Find(ReportViewerId);
            }
            else
            {
                return db.ReportViewer.AsNoTracking().FirstOrDefault(item => item.ReportViewerId == ReportViewerId);
            }
        }

        public Models.ReportViewer AddReportViewer(Models.ReportViewer ReportViewer)
        {
            using var db = _factory.CreateDbContext();
            db.ReportViewer.Add(ReportViewer);
            db.SaveChanges();
            return ReportViewer;
        }

        public Models.ReportViewer UpdateReportViewer(Models.ReportViewer ReportViewer)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(ReportViewer).State = EntityState.Modified;
            db.SaveChanges();
            return ReportViewer;
        }

        public void DeleteReportViewer(int ReportViewerId)
        {
            using var db = _factory.CreateDbContext();
            Models.ReportViewer ReportViewer = db.ReportViewer.Find(ReportViewerId);
            db.ReportViewer.Remove(ReportViewer);
            db.SaveChanges();
        }
    }
}
