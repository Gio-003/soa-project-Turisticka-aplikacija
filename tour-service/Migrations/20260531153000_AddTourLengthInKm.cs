using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tour_service.Migrations
{
    /// <inheritdoc />
    public partial class AddTourLengthInKm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LengthInKm",
                schema: "tour",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0d);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LengthInKm",
                schema: "tour",
                table: "Tours");
        }
    }
}
