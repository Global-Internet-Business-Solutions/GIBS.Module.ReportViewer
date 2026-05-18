using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Services;
using Oqtane.Shared;

namespace GIBS.Module.ReportViewer.Services
{

    public class ClientReportViewerService : ServiceBase, IReportViewerService
    {
        public ClientReportViewerService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("ReportViewer");

        public async Task<List<Models.ReportViewer>> GetReportViewersAsync(int ModuleId)
        {
            List<Models.ReportViewer> ReportViewers = await GetJsonAsync<List<Models.ReportViewer>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.ReportViewer>().ToList());
            return ReportViewers.OrderBy(item => item.Name).ToList();
        }

        public async Task<Models.ReportViewer> GetReportViewerAsync(int ReportViewerId, int ModuleId)
        {
            return await GetJsonAsync<Models.ReportViewer>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ReportViewerId}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.ReportViewer> AddReportViewerAsync(Models.ReportViewer ReportViewer)
        {
            return await PostJsonAsync<Models.ReportViewer>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, ReportViewer.ModuleId), ReportViewer);
        }

        public async Task<Models.ReportViewer> UpdateReportViewerAsync(Models.ReportViewer ReportViewer)
        {
            return await PutJsonAsync<Models.ReportViewer>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ReportViewer.ReportViewerId}", EntityNames.Module, ReportViewer.ModuleId), ReportViewer);
        }

        public async Task DeleteReportViewerAsync(int ReportViewerId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{ReportViewerId}/{ModuleId}", EntityNames.Module, ModuleId));
        }
    }
}
