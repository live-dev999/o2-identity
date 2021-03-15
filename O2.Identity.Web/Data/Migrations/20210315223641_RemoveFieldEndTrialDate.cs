using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace O2.Identity.Web.Data.Migrations
{
    public partial class RemoveFieldEndTrialDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TrialEndDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrialStartDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrialEndDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TrialStartDate",
                table: "AspNetUsers");
        }
    }
}
