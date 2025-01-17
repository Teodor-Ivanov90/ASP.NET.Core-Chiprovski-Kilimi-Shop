﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ChiprovciCarpetsShop.Data.Migrations
{
    public partial class AddedIsPublicColumnInProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Products");
        }
    }
}
