using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyQuotationManager.Migrations
{
    public partial class UpdateProductCompositionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubProductId",
                table: "ProductCompositions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubProductId",
                table: "ProductCompositions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
