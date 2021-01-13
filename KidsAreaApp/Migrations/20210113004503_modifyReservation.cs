using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class modifyReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Reservations");

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "Reservations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Reservations");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
