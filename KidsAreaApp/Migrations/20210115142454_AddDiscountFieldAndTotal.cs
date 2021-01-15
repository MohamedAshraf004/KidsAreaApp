using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class AddDiscountFieldAndTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "Reservations",
                newName: "TotatCost");

            migrationBuilder.AddColumn<double>(
                name: "CostAfterDiscount",
                table: "Reservations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostAfterDiscount",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "TotatCost",
                table: "Reservations",
                newName: "Cost");
        }
    }
}
