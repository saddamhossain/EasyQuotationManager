using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyQuotationManager.Migrations
{
    public partial class ProductCompositionModelUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubProductName",
                table: "ProductCompositions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubProductName",
                table: "ProductCompositions");
        }
    }
}
