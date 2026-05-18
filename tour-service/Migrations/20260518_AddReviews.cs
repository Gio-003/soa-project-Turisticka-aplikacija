using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tour_service.Migrations
{
    /// <inheritdoc />
    public partial class AddReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Images = table.Column<string>(type: "text", nullable: false, defaultValue: "[]"),
                    ReviewerName = table.Column<string>(type: "text", nullable: true),
                    ReviewerProfilePicture = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TourId",
                schema: "tour",
                table: "Reviews",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerUserId",
                schema: "tour",
                table: "Reviews",
                column: "ReviewerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "tour");
        }
    }
}
