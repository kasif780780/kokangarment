using Microsoft.EntityFrameworkCore.Migrations;

namespace BasePackageModule1.Migrations
{
    public partial class DiscountAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "DiscountAmount",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "DiscountedPrice",
                table: "Products",
                type: "float",
                nullable: true);
        }
    }
}
