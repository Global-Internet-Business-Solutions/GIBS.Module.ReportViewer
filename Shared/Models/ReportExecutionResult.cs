using System;
using System.Collections.Generic;

namespace GIBS.Module.ReportViewer.Models
{
    public class ReportExecutionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string ViewMode { get; set; }
        public List<string> Columns { get; set; } = new();
        public List<Dictionary<string, string>> Rows { get; set; } = new();
        public string RenderedTemplate { get; set; }
        public DateTime GeneratedOnUtc { get; set; }
    }
}
