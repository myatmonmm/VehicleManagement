using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedIconNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "bi-car-front-fill");

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "bi-taxi-front-fill");

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: "bi-truck-front-fill");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Icon",
                value: "car-light");

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Icon",
                value: "car-medium");

            migrationBuilder.UpdateData(
                table: "VehicleCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Icon",
                value: "truck");
        }
    }
}
