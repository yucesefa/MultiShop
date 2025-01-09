using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiShop.Discount.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidDate",
                table: "Coupons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "ValidDate",
                table: "Coupons");
        }
    }
}
