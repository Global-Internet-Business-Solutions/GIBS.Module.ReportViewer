using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Oqtane.Services;
using GIBS.Module.ReportViewer.Services;

namespace GIBS.Module.ReportViewer.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IReportViewerService)))
            {
                services.AddScoped<IReportViewerService, ClientReportViewerService>();
            }
        }
    }
}
