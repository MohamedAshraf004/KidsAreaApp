using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class LastScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Receipts_ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReceiptSerialKey",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Reservations",
                newName: "SerialKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SerialKey",
                table: "Reservations",
                newName: "ReservationId");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptSerialKey",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    SerialKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.SerialKey);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReceiptSerialKey",
                table: "Reservations",
                column: "ReceiptSerialKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Receipts_ReceiptSerialKey",
                table: "Reservations",
                column: "ReceiptSerialKey",
                principalTable: "Receipts",
                principalColumn: "SerialKey",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
