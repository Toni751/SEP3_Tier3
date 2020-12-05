using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP3_Tier3.Migrations
{
    public partial class RemovedOwnerFromExercise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Users_OwnerId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_OwnerId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Exercise");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Exercise",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_OwnerId",
                table: "Exercise",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Users_OwnerId",
                table: "Exercise",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
