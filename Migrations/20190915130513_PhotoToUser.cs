using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.Apı.Migrations
{
    public partial class PhotoToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UsersId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UsersId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Photos");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Photos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UserId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photos");

            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "Photos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UsersId",
                table: "Photos",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UsersId",
                table: "Photos",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
