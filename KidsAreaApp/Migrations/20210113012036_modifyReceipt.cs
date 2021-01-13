using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class modifyReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Receipt_Receiptid",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_Receiptid",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "Receiptid",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Receiptid",
                table: "Receipt");

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiptSerialKey",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt",
                column: "SerialKey");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReceiptSerialKey",
                table: "Reservations",
                column: "ReceiptSerialKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Receipt_ReceiptSerialKey",
                table: "Reservations",
                column: "ReceiptSerialKey",
                principalTable: "Receipt",
                principalColumn: "SerialKey",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Receipt_ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "Receiptid",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Receiptid",
                table: "Receipt",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipt",
                table: "Receipt",
                column: "Receiptid");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Receiptid",
                table: "Reservations",
                column: "Receiptid");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Receipt_Receiptid",
                table: "Reservations",
                column: "Receiptid",
                principalTable: "Receipt",
                principalColumn: "Receiptid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
