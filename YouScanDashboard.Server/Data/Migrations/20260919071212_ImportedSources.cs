using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouScanDashboard.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImportedSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "imported_sources",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_imported_sources", x => x.key);
                });

            // Existing databases recorded the imported tables in datasets.source_key.
            migrationBuilder.Sql("""
                INSERT INTO imported_sources (key)
                SELECT DISTINCT source_key FROM datasets WHERE source_key IS NOT NULL
                ON CONFLICT DO NOTHING;
                """);

            // Datasets of deleted widgets were kept for that purpose and have no owner any more.
            migrationBuilder.Sql("""
                DELETE FROM datasets d
                WHERE NOT EXISTS (SELECT 1 FROM widgets w WHERE w.dataset_id = d.id);
                """);

            migrationBuilder.DropIndex(
                name: "ix_datasets_source_key",
                table: "datasets");

            migrationBuilder.DropColumn(
                name: "source_key",
                table: "datasets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imported_sources");

            migrationBuilder.AddColumn<string>(
                name: "source_key",
                table: "datasets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_datasets_source_key",
                table: "datasets",
                column: "source_key",
                unique: true);
        }
    }
}