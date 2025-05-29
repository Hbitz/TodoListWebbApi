using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoWebApi.Migrations
{
    /// <inheritdoc />
    public partial class SwitchBackToUserModelTemporarily : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_UserId1",
                table: "TodoItems");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "TodoItems",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_UserId1",
                table: "TodoItems",
                newName: "IX_TodoItems_ApplicationUserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Categories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_UserId",
                table: "TodoItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId1",
                table: "Categories",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId1",
                table: "Categories",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_ApplicationUserId",
                table: "TodoItems",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Users_UserId",
                table: "TodoItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId1",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_ApplicationUserId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Users_UserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_UserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId1",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "TodoItems",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItems_ApplicationUserId",
                table: "TodoItems",
                newName: "IX_TodoItems_UserId1");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_UserId1",
                table: "TodoItems",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
