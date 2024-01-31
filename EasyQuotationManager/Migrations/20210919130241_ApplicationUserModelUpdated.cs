using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyQuotationManager.Migrations
{
    public partial class ApplicationUserModelUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamLeaderAccessToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamLeaderAuthCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamLeaderRefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TeamLeaderTokenCreatedUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TeamLeaderTokenExpireInUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamLeaderAccessToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamLeaderAuthCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamLeaderRefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamLeaderTokenCreatedUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamLeaderTokenExpireInUtc",
                table: "AspNetUsers");
        }
    }
}
