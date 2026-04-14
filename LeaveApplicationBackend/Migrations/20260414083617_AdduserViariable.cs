using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class AdduserViariable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Team",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApproverEmail",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Team",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "ApproverEmail",
                table: "LeaveApplications");
        }
    }
}
