using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace O2.Identity.Web.Data.Migrations
{
    public partial class addedCustomFieldIsSpecialist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialist",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpecialist",
                table: "AspNetUsers");
        }
    }
}
