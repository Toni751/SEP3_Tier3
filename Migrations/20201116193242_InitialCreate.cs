using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace SEP3_Tier3.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Street = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegularUser",
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
                    Discriminator = table.Column<string>(nullable: false),
                    AddressId = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Diet",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RegularUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diet_RegularUser_RegularUserId",
                        column: x => x.RegularUserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RegularUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercise_RegularUser_RegularUserId",
                        column: x => x.RegularUserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    FirstUserId = table.Column<int>(nullable: false),
                    SecondUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => new { x.FirstUserId, x.SecondUserId });
                    table.ForeignKey(
                        name: "FK_Friendships_RegularUser_FirstUserId",
                        column: x => x.FirstUserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friendships_RegularUser_SecondUserId",
                        column: x => x.SecondUserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Calories = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    RegularUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meal_RegularUser_RegularUserId",
                        column: x => x.RegularUserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    SenderId = table.Column<int>(nullable: false),
                    ReceiverId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => new { x.SenderId, x.ReceiverId });
                    table.ForeignKey(
                        name: "FK_Messages_RegularUser_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_RegularUser_SenderId",
                        column: x => x.SenderId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageRatings",
                columns: table => new
                {
                    PageId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageRatings", x => new { x.PageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PageRatings_RegularUser_PageId",
                        column: x => x.PageId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageRatings_RegularUser_UserId",
                        column: x => x.UserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    OwnerId = table.Column<int>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_RegularUser_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Training",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    RegularUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Training_RegularUser_RegularUserId",
                        column: x => x.RegularUserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserActions",
                columns: table => new
                {
                    SenderId = table.Column<int>(nullable: false),
                    ReceiverId = table.Column<int>(nullable: false),
                    IsFriendRequest = table.Column<bool>(nullable: false),
                    IsReport = table.Column<bool>(nullable: false),
                    IsShareTrainings = table.Column<bool>(nullable: false),
                    IsShareDiets = table.Column<bool>(nullable: false),
                    IsFollowPage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActions", x => new { x.SenderId, x.ReceiverId });
                    table.ForeignKey(
                        name: "FK_UserActions_RegularUser_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActions_RegularUser_SenderId",
                        column: x => x.SenderId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DietMeals",
                columns: table => new
                {
                    MealId = table.Column<int>(nullable: false),
                    DietId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietMeals", x => new { x.DietId, x.MealId });
                    table.ForeignKey(
                        name: "FK_DietMeals_Diet_DietId",
                        column: x => x.DietId,
                        principalTable: "Diet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DietMeals_Meal_MealId",
                        column: x => x.MealId,
                        principalTable: "Meal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<int>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_RegularUser_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostActions",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    IsLike = table.Column<bool>(nullable: false),
                    IsReport = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostActions", x => new { x.PostId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PostActions_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostActions_RegularUser_UserId",
                        column: x => x.UserId,
                        principalTable: "RegularUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingExercises",
                columns: table => new
                {
                    TrainingId = table.Column<int>(nullable: false),
                    ExerciseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingExercises", x => new { x.TrainingId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_TrainingExercises_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingExercises_Training_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Training",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OwnerId",
                table: "Comment",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Diet_RegularUserId",
                table: "Diet",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DietMeals_MealId",
                table: "DietMeals",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_RegularUserId",
                table: "Exercise",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_SecondUserId",
                table: "Friendships",
                column: "SecondUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meal_RegularUserId",
                table: "Meal",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_PageRatings_UserId",
                table: "PageRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostActions_UserId",
                table: "PostActions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OwnerId",
                table: "Posts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RegularUser_AddressId",
                table: "RegularUser",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Training_RegularUserId",
                table: "Training",
                column: "RegularUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingExercises_ExerciseId",
                table: "TrainingExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActions_ReceiverId",
                table: "UserActions",
                column: "ReceiverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "DietMeals");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "PageRatings");

            migrationBuilder.DropTable(
                name: "PostActions");

            migrationBuilder.DropTable(
                name: "TrainingExercises");

            migrationBuilder.DropTable(
                name: "UserActions");

            migrationBuilder.DropTable(
                name: "Diet");

            migrationBuilder.DropTable(
                name: "Meal");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropTable(
                name: "Training");

            migrationBuilder.DropTable(
                name: "RegularUser");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
