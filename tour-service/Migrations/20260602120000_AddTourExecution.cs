using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tour_service.Migrations
{
    /// <inheritdoc />
    public partial class AddTourExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourExecutions",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    TouristId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AbandonedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartLatitude = table.Column<double>(type: "double precision", nullable: false),
                    StartLongitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourExecutions_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tour",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompletedKeyPoints",
                schema: "tour",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TourExecutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyPointId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedKeyPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompletedKeyPoints_TourExecutions_TourExecutionId",
                        column: x => x.TourExecutionId,
                        principalSchema: "tour",
                        principalTable: "TourExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedKeyPoints_TourExecutionId",
                schema: "tour",
                table: "CompletedKeyPoints",
                column: "TourExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TourExecutions_TourId",
                schema: "tour",
                table: "TourExecutions",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedKeyPoints",
                schema: "tour");

            migrationBuilder.DropTable(
                name: "TourExecutions",
                schema: "tour");
        }
    }
}
