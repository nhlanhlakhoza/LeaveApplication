using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class Addedforeignkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalances_UserId",
                table: "LeaveBalances");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "LeaveBalances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_UserId",
                table: "LeaveBalances",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances");

            migrationBuilder.DropIndex(
                name: "IX_LeaveBalances_UserId",
                table: "LeaveBalances");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "LeaveBalances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_UserId",
                table: "LeaveBalances",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveBalances_Users_UserId",
                table: "LeaveBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
