using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQCDMS.Migrations
{
    /// <inheritdoc />
    public partial class AddIsImportantField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Documents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Documents");
        }
    }
}
