using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceDepartment.EvidenceManager.Infra.Database.Migrations
{
    /// <inheritdoc />
    public partial class Adding_Name_Officer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Officers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Officers");
        }
    }
}
