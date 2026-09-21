using Oqtane.Models;
using Oqtane.Modules;

namespace GIBS.Module.ReportViewer
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "ReportViewer",
            Description = "GIBS Report Viewer Module for Oqtane",
            Version = "1.0.2",
            ServerManagerType = "GIBS.Module.ReportViewer.Manager.ReportViewerManager, GIBS.Module.ReportViewer.Server.Oqtane",
            ReleaseVersions = "1.0.0,1.0.1,1.0.2",
            Dependencies = "GIBS.Module.ReportViewer.Shared.Oqtane, Oqtane.Licensing.Client.Oqtane, Oqtane.Licensing.Shared.Oqtane",
            PackageName = "GIBS.Module.ReportViewer" 
        };
    }
}
