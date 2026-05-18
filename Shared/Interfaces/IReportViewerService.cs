using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.ReportViewer.Services
{
    public interface IReportViewerService 
    {
        Task<List<Models.ReportViewer>> GetReportViewersAsync(int ModuleId);

        Task<Models.ReportViewer> GetReportViewerAsync(int ReportViewerId, int ModuleId);

        Task<Models.ReportViewer> AddReportViewerAsync(Models.ReportViewer ReportViewer);

        Task<Models.ReportViewer> UpdateReportViewerAsync(Models.ReportViewer ReportViewer);

        Task DeleteReportViewerAsync(int ReportViewerId, int ModuleId);

        Task<Models.ReportExecutionResult> ExecuteReportAsync(int moduleId, bool bypassCache);
    }
}
