using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKbkm.Migrations
{
    /// <inheritdoc />
    public partial class DockerMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountCreate",
                table: "AccountCreate");

            migrationBuilder.RenameTable(
                name: "AccountCreate",
                newName: "AccountCreates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountCreates",
                table: "AccountCreates",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountCreates",
                table: "AccountCreates");

            migrationBuilder.RenameTable(
                name: "AccountCreates",
                newName: "AccountCreate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountCreate",
                table: "AccountCreate",
                column: "id");
        }
    }
}
