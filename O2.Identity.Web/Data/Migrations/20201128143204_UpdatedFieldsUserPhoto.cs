using Microsoft.EntityFrameworkCore.Migrations;

namespace O2.Identity.Web.Data.Migrations
{
    public partial class UpdatedFieldsUserPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UserId1",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "ProfilePhoto",
                table: "AspNetUsers",
                newName: "ProfilePhoto");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "Photos",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UserId1",
                table: "Photos",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UserId1",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "ProfilePhoto",
                table: "AspNetUsers",
                newName: "ProfilePhoto");

            migrationBuilder.AlterColumn<string>(
                name: "UserId1",
                table: "Photos",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UserId1",
                table: "Photos",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
