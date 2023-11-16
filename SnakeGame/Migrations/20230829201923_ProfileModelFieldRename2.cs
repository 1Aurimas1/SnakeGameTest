using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnakeGame.Migrations
{
    /// <inheritdoc />
    public partial class ProfileModelFieldRename2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HighScore",
                table: "Profiles",
                newName: "Highscore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Highscore",
                table: "Profiles",
                newName: "HighScore");
        }
    }
}
