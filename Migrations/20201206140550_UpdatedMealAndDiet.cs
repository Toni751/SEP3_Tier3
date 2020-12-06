using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP3_Tier3.Migrations
{
    public partial class UpdatedMealAndDiet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meal_Users_OwnerId",
                table: "Meal");

            migrationBuilder.DropIndex(
                name: "IX_Meal_OwnerId",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Meal");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Diet",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Diet");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Meal",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meal_OwnerId",
                table: "Meal",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_Users_OwnerId",
                table: "Meal",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
