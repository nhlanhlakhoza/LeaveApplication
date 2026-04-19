using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmployeeEmailFromLeaveBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "EmployeeEmail",
                table: "LeaveBalances");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeEmail",
                table: "LeaveBalances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.DropColumn(
        name: "EmployeeEmail",
        table: "LeaveBalances");
        }
    }
}
