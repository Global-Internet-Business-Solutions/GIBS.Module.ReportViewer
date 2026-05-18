using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using GIBS.Module.ReportViewer.Repository;
using GIBS.Module.ReportViewer.Services;

namespace GIBS.Module.ReportViewer.Startup
{
    public class ServerStartup : IServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not implemented
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // not implemented
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IReportViewerService, ServerReportViewerService>();
            services.AddDbContextFactory<ReportViewerContext>(opt => { }, ServiceLifetime.Transient);
        }
    }
}
