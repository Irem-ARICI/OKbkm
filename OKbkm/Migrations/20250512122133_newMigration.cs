using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKbkm.Migrations
{
    /// <inheritdoc />
    public partial class newMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory");

            migrationBuilder.RenameTable(
                name: "TransactionHistory",
                newName: "TransactionHistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionHistories",
                table: "TransactionHistories",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionHistories",
                table: "TransactionHistories");

            migrationBuilder.RenameTable(
                name: "TransactionHistories",
                newName: "TransactionHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory",
                column: "Id");
        }
    }
}
