using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class AddReceiotClassToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Receipt_ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt");

            migrationBuilder.RenameTable(
                name: "Receipt",
                newName: "Receipts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts",
                column: "SerialKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Receipts_ReceiptSerialKey",
                table: "Reservations",
                column: "ReceiptSerialKey",
                principalTable: "Receipts",
                principalColumn: "SerialKey",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Receipts_ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts");

            migrationBuilder.RenameTable(
                name: "Receipts",
                newName: "Receipt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt",
                column: "SerialKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Receipt_ReceiptSerialKey",
                table: "Reservations",
                column: "ReceiptSerialKey",
                principalTable: "Receipt",
                principalColumn: "SerialKey",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
