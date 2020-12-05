using Microsoft.EntityFrameworkCore.Migrations;

namespace SEP3_Tier3.Migrations
{
    public partial class UpdatedTrainingAndExercise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Exercise");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Training",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Training",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Training");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Exercise",
                type: "text",
                nullable: true);
        }
    }
}
