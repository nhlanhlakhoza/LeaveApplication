using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class newVariableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "LeaveApplications");
        }
    }
}
