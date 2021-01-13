using Microsoft.EntityFrameworkCore.Migrations;

namespace KidsAreaApp.Migrations
{
    public partial class AddQrCodePathRoReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCodePath",
                table: "Receipt",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodePath",
                table: "Receipt");
        }
    }
}
