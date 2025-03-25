using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKbkm.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceToAccountCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ACreates",
                table: "ACreates");

            migrationBuilder.RenameTable(
                name: "ACreates",
                newName: "AccountCreate");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AccountCreate",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountCreate",
                table: "AccountCreate",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountCreate",
                table: "AccountCreate");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AccountCreate");

            migrationBuilder.RenameTable(
                name: "AccountCreate",
                newName: "ACreates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ACreates",
                table: "ACreates",
                column: "id");
        }
    }
}
