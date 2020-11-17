using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace SEP3_Tier3.Migrations
{
    public partial class UpdatedUserAndMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_RegularUser_OwnerId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Diet_RegularUser_RegularUserId",
                table: "Diet");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_RegularUser_RegularUserId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_RegularUser_FirstUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_RegularUser_SecondUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Meal_RegularUser_RegularUserId",
                table: "Meal");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_RegularUser_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_RegularUser_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_PageRatings_RegularUser_PageId",
                table: "PageRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PageRatings_RegularUser_UserId",
                table: "PageRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PostActions_RegularUser_UserId",
                table: "PostActions");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_RegularUser_OwnerId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_RegularUser_RegularUserId",
                table: "Training");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActions_RegularUser_ReceiverId",
                table: "UserActions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActions_RegularUser_SenderId",
                table: "UserActions");

            migrationBuilder.DropTable(
                name: "RegularUser");

            migrationBuilder.DropIndex(
                name: "IX_Training_RegularUserId",
                table: "Training");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Meal_RegularUserId",
                table: "Meal");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_RegularUserId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Diet_RegularUserId",
                table: "Diet");

            migrationBuilder.DropColumn(
                name: "RegularUserId",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "RegularUserId",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "RegularUserId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "RegularUserId",
                table: "Diet");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Training",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Meal",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Exercise",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Diet",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_UserId",
                table: "Training",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Meal_UserId",
                table: "Meal",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_UserId",
                table: "Exercise",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Diet_UserId",
                table: "Diet",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_OwnerId",
                table: "Comment",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Diet_Users_UserId",
                table: "Diet",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Users_UserId",
                table: "Exercise",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_FirstUserId",
                table: "Friendships",
                column: "FirstUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_SecondUserId",
                table: "Friendships",
                column: "SecondUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_Users_UserId",
                table: "Meal",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageRatings_Users_PageId",
                table: "PageRatings",
                column: "PageId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageRatings_Users_UserId",
                table: "PageRatings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostActions_Users_UserId",
                table: "PostActions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_OwnerId",
                table: "Posts",
                column: "OwnerId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_UserActions_Users_ReceiverId",
                table: "UserActions",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActions_Users_SenderId",
                table: "UserActions",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_OwnerId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Diet_Users_UserId",
                table: "Diet");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Users_UserId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_FirstUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_SecondUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Meal_Users_UserId",
                table: "Meal");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_PageRatings_Users_PageId",
                table: "PageRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PageRatings_Users_UserId",
                table: "PageRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_PostActions_Users_UserId",
                table: "PostActions");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_OwnerId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_Users_UserId",
                table: "Training");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActions_Users_ReceiverId",
                table: "UserActions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActions_Users_SenderId",
                table: "UserActions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Training_UserId",
                table: "Training");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Meal_UserId",
                table: "Meal");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_UserId",
                table: "Exercise");

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
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Diet");

            migrationBuilder.AddColumn<int>(
                name: "RegularUserId",
                table: "Training",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Messages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "RegularUserId",
                table: "Meal",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegularUserId",
                table: "Exercise",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegularUserId",
                table: "Diet",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                columns: new[] { "SenderId", "ReceiverId" });

            migrationBuilder.CreateTable(
                name: "RegularUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegularUser_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_RegularUserId",
                table: "Training",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meal_RegularUserId",
                table: "Meal",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_RegularUserId",
                table: "Exercise",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Diet_RegularUserId",
                table: "Diet",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegularUser_AddressId",
                table: "RegularUser",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_RegularUser_OwnerId",
                table: "Comment",
                column: "OwnerId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Diet_RegularUser_RegularUserId",
                table: "Diet",
                column: "RegularUserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_RegularUser_RegularUserId",
                table: "Exercise",
                column: "RegularUserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_RegularUser_FirstUserId",
                table: "Friendships",
                column: "FirstUserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_RegularUser_SecondUserId",
                table: "Friendships",
                column: "SecondUserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_RegularUser_RegularUserId",
                table: "Meal",
                column: "RegularUserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_RegularUser_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_RegularUser_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageRatings_RegularUser_PageId",
                table: "PageRatings",
                column: "PageId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageRatings_RegularUser_UserId",
                table: "PageRatings",
                column: "UserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostActions_RegularUser_UserId",
                table: "PostActions",
                column: "UserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_RegularUser_OwnerId",
                table: "Posts",
                column: "OwnerId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_RegularUser_RegularUserId",
                table: "Training",
                column: "RegularUserId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActions_RegularUser_ReceiverId",
                table: "UserActions",
                column: "ReceiverId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActions_RegularUser_SenderId",
                table: "UserActions",
                column: "SenderId",
                principalTable: "RegularUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
