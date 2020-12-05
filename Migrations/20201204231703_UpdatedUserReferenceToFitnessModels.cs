using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP3_Tier3.Migrations
{
    public partial class UpdatedUserReferenceToFitnessModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diet_Users_UserId",
                table: "Diet");

            migrationBuilder.DropForeignKey(
                name: "FK_Meal_Users_UserId",
                table: "Meal");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_Users_UserId",
                table: "Training");

            migrationBuilder.DropIndex(
                name: "IX_Training_UserId",
                table: "Training");

            migrationBuilder.DropIndex(
                name: "IX_Meal_UserId",
                table: "Meal");

            migrationBuilder.DropIndex(
                name: "IX_Diet_UserId",
                table: "Diet");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Diet");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Training",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Meal",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Exercise",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Diet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Training_OwnerId",
                table: "Training",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Meal_OwnerId",
                table: "Meal",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_OwnerId",
                table: "Exercise",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Diet_OwnerId",
                table: "Diet",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diet_Users_OwnerId",
                table: "Diet",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Users_OwnerId",
                table: "Exercise",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_Users_OwnerId",
                table: "Meal",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Users_OwnerId",
                table: "Training",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diet_Users_OwnerId",
                table: "Diet");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Users_OwnerId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_Meal_Users_OwnerId",
                table: "Meal");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_Users_OwnerId",
                table: "Training");

            migrationBuilder.DropIndex(
                name: "IX_Training_OwnerId",
                table: "Training");

            migrationBuilder.DropIndex(
                name: "IX_Meal_OwnerId",
                table: "Meal");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_OwnerId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Diet_OwnerId",
                table: "Diet");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Diet");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Training",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Meal",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Diet",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Training_UserId",
                table: "Training",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meal_UserId",
                table: "Meal",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Diet_UserId",
                table: "Diet",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diet_Users_UserId",
                table: "Diet",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_Users_UserId",
                table: "Meal",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_Users_UserId",
                table: "Training",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
