using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectReview.Migrations
{
    /// <inheritdoc />
    public partial class update_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_User",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionGroup_User",
                table: "PermissionGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Position_User",
                table: "Position");

            migrationBuilder.DropForeignKey(
                name: "FK_Rank_User",
                table: "Rank");

            migrationBuilder.DropForeignKey(
                name: "FK_User_CreateUser",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CreateUserId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Rank_CreateUserId",
                table: "Rank");

            migrationBuilder.DropIndex(
                name: "IX_Position_CreateUserId",
                table: "Position");

            migrationBuilder.DropIndex(
                name: "IX_PermissionGroup_CreateUserId",
                table: "PermissionGroup");

            migrationBuilder.DropIndex(
                name: "IX_Department_CreateUserId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "CreateUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreateUserId",
                table: "Rank");

            migrationBuilder.DropColumn(
                name: "CreateUserId",
                table: "Position");

            migrationBuilder.DropColumn(
                name: "CreateUserId",
                table: "PermissionGroup");

            migrationBuilder.DropColumn(
                name: "CreateUserId",
                table: "Department");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreateUserId",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreateUserId",
                table: "Rank",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreateUserId",
                table: "Position",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreateUserId",
                table: "PermissionGroup",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreateUserId",
                table: "Department",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_User_CreateUserId",
                table: "User",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rank_CreateUserId",
                table: "Rank",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_CreateUserId",
                table: "Position",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionGroup_CreateUserId",
                table: "PermissionGroup",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CreateUserId",
                table: "Department",
                column: "CreateUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_User",
                table: "Department",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionGroup_User",
                table: "PermissionGroup",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Position_User",
                table: "Position",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rank_User",
                table: "Rank",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_CreateUser",
                table: "User",
                column: "CreateUserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
