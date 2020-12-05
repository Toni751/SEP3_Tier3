using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP3_Tier3.Migrations
{
    public partial class RevertingLastMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Training_TrainingId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_TrainingId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "TrainingId",
                table: "Exercise");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrainingId",
                table: "Exercise",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_TrainingId",
                table: "Exercise",
                column: "TrainingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Training_TrainingId",
                table: "Exercise",
                column: "TrainingId",
                principalTable: "Training",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
