using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyQuotationManager.Migrations
{
    public partial class LicenseExpirationDatePropertyInApplicationUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpirationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseExpirationDate",
                table: "AspNetUsers");
        }
    }
}
