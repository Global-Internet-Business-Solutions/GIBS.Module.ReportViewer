namespace GIBS.Module.ReportViewer.Models
{
    public class ReportViewerSettings
    {
        public const string DataSourceLocal = "Local";
        public const string DataSourceRemote = "Remote";
        public const string ViewModeTable = "Table";
        public const string ViewModeTemplate = "Template";

        public string DataSource { get; set; } = DataSourceLocal;
        public string ConnectionString { get; set; }
        public string SqlQuery { get; set; }
        public string ViewMode { get; set; } = ViewModeTable;
        public string TemplateHeader { get; set; }
        public string Template { get; set; }
        public string TemplateFooter { get; set; }
        public int CacheDurationMinutes { get; set; } = 5;
    }
}
