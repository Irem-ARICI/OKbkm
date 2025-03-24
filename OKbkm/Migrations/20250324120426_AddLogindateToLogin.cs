using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKbkm.Migrations
{
    /// <inheritdoc />
    public partial class AddLogindateToLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LoginDate",
                table: "Logins",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginDate",
                table: "Logins");
        }
    }
}
