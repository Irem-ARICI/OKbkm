using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKbkm.Migrations
{
    /// <inheritdoc />
    public partial class TransactionHistoryUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_THistories",
                table: "THistories");

            migrationBuilder.RenameTable(
                name: "THistories",
                newName: "TransactionHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionHistory",
                table: "TransactionHistory");

            migrationBuilder.RenameTable(
                name: "TransactionHistory",
                newName: "THistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_THistories",
                table: "THistories",
                column: "Id");
        }
    }
}
