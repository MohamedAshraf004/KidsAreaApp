using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class AddDiscountField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Reservations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Reservations");
        }
    }
}
