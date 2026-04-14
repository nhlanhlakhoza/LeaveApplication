using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveApplicationBackend.Migrations
{
    /// <inheritdoc />
    public partial class AdduserViariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "LeaveApplications");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "LeaveApplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "LeaveApplications");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "LeaveApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
