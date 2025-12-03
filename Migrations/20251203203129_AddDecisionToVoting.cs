using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingService.Migrations
{
    /// <inheritdoc />
    public partial class AddDecisionToVoting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Decision",
                table: "Votings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decision",
                table: "Votings");
        }
    }
}
