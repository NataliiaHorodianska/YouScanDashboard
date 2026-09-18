using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouScanDashboard.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "datasets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_key = table.Column<string>(type: "text", nullable: true),
                    data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_datasets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "widgets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    dataset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_widgets", x => x.id);
                    table.ForeignKey(
                        name: "fk_widgets_datasets_dataset_id",
                        column: x => x.dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_datasets_source_key",
                table: "datasets",
                column: "source_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_widgets_dataset_id",
                table: "widgets",
                column: "dataset_id");

            migrationBuilder.CreateIndex(
                name: "ix_widgets_position",
                table: "widgets",
                column: "position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "widgets");

            migrationBuilder.DropTable(
                name: "datasets");
        }
    }
}
