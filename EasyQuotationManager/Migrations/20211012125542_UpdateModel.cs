using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyQuotationManager.Migrations
{
    public partial class UpdateModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombinedProductId",
                table: "ProductCompositions");

            migrationBuilder.AddColumn<string>(
                name: "CombinedProductIdOrName",
                table: "ProductCompositions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombinedProductIdOrName",
                table: "ProductCompositions");

            migrationBuilder.AddColumn<int>(
                name: "CombinedProductId",
                table: "ProductCompositions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
