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
            Version = "1.0.0",
            ServerManagerType = "GIBS.Module.ReportViewer.Manager.ReportViewerManager, GIBS.Module.ReportViewer.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "GIBS.Module.ReportViewer.Shared.Oqtane",
            PackageName = "GIBS.Module.ReportViewer" 
        };
    }
}
