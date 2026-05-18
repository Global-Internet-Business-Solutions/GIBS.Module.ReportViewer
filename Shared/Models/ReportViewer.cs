using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace GIBS.Module.ReportViewer.Models
{
    [Table("GIBSReportViewer")]
    public class ReportViewer : ModelBase
    {
        [Key]
        public int ReportViewerId { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
    }
}
