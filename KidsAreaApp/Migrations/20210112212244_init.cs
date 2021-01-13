using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Receipt",
                columns: table => new
                {
                    Receiptid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BarCode = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt", x => x.Receiptid);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Receiptid = table.Column<int>(type: "int", nullable: true),
                    StartReservationTme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndReservationTme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Receipt_Receiptid",
                        column: x => x.Receiptid,
                        principalTable: "Receipt",
                        principalColumn: "Receiptid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Receiptid",
                table: "Reservations",
                column: "Receiptid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Receipt");
        }
    }
}
