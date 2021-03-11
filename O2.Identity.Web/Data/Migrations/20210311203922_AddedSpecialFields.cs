using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace O2.Identity.Web.Data.Migrations
{
    public partial class AddedSpecialFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageProficiency",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Education",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LanguageProficiency",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "AspNetUsers");
        }
    }
}
