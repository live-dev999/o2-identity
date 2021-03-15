using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace O2.Identity.Web.Data.Migrations
{
    public partial class RemoveFieldEndTrialDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trial",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Trial",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
