using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AISA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScrapingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Reviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleMapsUrl",
                table: "BusinessProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TripAdvisorUrl",
                table: "BusinessProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "GoogleMapsUrl",
                table: "BusinessProfiles");

            migrationBuilder.DropColumn(
                name: "TripAdvisorUrl",
                table: "BusinessProfiles");
        }
    }
}
