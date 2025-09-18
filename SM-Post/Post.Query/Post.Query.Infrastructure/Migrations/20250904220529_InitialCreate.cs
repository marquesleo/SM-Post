using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Post.Query.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Author = table.Column<string>(type: "longtext", nullable: false),
                    DatePosted = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Username = table.Column<string>(type: "longtext", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: false),
                    Edited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PostId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Post");
        }
    }
}
