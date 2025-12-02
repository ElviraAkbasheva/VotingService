using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingService.Migrations
{
    /// <inheritdoc />
    public partial class AddVoteWeightToOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VoteWeight",
                table: "Owners",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoteWeight",
                table: "Owners");
        }
    }
}
