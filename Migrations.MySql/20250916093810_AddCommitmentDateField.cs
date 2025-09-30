using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CQCDMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitmentDateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommitmentDate",
                table: "Documents",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommitmentDate",
                table: "Documents");
        }
    }
}
