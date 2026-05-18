using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Enums;
using Oqtane.Repository;
using GIBS.Module.ReportViewer.Repository;
using System.Threading.Tasks;

namespace GIBS.Module.ReportViewer.Manager
{
    public class ReportViewerManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly IReportViewerRepository _ReportViewerRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

        public ReportViewerManager(IReportViewerRepository ReportViewerRepository, IDBContextDependencies DBContextDependencies)
        {
            _ReportViewerRepository = ReportViewerRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new ReportViewerContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new ReportViewerContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            string content = "";
            List<Models.ReportViewer> ReportViewers = _ReportViewerRepository.GetReportViewers(module.ModuleId).ToList();
            if (ReportViewers != null)
            {
                content = JsonSerializer.Serialize(ReportViewers);
            }
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            List<Models.ReportViewer> ReportViewers = null;
            if (!string.IsNullOrEmpty(content))
            {
                ReportViewers = JsonSerializer.Deserialize<List<Models.ReportViewer>>(content);
            }
            if (ReportViewers != null)
            {
                foreach(var ReportViewer in ReportViewers)
                {
                    _ReportViewerRepository.AddReportViewer(new Models.ReportViewer { ModuleId = module.ModuleId, Name = ReportViewer.Name });
                }
            }
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
           var searchContentList = new List<SearchContent>();

           foreach (var ReportViewer in _ReportViewerRepository.GetReportViewers(pageModule.ModuleId))
           {
               if (ReportViewer.ModifiedOn >= lastIndexedOn)
               {
                   searchContentList.Add(new SearchContent
                   {
                       EntityName = "GIBSReportViewer",
                       EntityId = ReportViewer.ReportViewerId.ToString(),
                       Title = ReportViewer.Name,
                       Body = ReportViewer.Name,
                       ContentModifiedBy = ReportViewer.ModifiedBy,
                       ContentModifiedOn = ReportViewer.ModifiedOn
                   });
               }
           }

           return Task.FromResult(searchContentList);
        }
    }
}
