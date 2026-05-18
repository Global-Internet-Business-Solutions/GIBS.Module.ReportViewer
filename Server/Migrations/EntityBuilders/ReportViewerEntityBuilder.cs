using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.ReportViewer.Migrations.EntityBuilders
{
    public class ReportViewerEntityBuilder : AuditableBaseEntityBuilder<ReportViewerEntityBuilder>
    {
        private const string _entityTableName = "GIBSReportViewer";
        private readonly PrimaryKey<ReportViewerEntityBuilder> _primaryKey = new("PK_GIBSReportViewer", x => x.ReportViewerId);
        private readonly ForeignKey<ReportViewerEntityBuilder> _moduleForeignKey = new("FK_GIBSReportViewer_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public ReportViewerEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override ReportViewerEntityBuilder BuildTable(ColumnsBuilder table)
        {
            ReportViewerId = AddAutoIncrementColumn(table,"ReportViewerId");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            Name = AddMaxStringColumn(table,"Name");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> ReportViewerId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Name { get; set; }
    }
}
