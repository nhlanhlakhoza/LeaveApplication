using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class Addedforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LeaveBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LeaveApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_UserId",
                table: "LeaveBalances",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplications_UserId",
                table: "LeaveApplications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApplications_Users_UserId",
                table: "LeaveApplications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_LeaveApplications_Users_UserId",
                table: "LeaveApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalances_UserId",
                table: "LeaveBalances");

            migrationBuilder.DropIndex(
                name: "IX_LeaveApplications_UserId",
                table: "LeaveApplications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LeaveBalances");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LeaveApplications");
        }
    }
}
