using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tour_service.Migrations
{
    /// <inheritdoc />
    public partial class AddTourDurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourDurations",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransportType = table.Column<int>(type: "integer", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourDurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourDurations_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourDurations_TourId",
                schema: "tour",
                table: "TourDurations",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourDurations",
                schema: "tour");
        }
    }
}
