using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdApartmentIdHouseIdToOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApartmentId",
                table: "Owners",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "HouseId",
                table: "Owners",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Owners",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "HouseId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Owners");
        }
    }
}
