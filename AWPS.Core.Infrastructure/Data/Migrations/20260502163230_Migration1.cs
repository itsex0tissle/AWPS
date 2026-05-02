using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWPS.Core.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorsData_DeviceProfileId",
                table: "SensorsData");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "DeviceProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SensorsData_DeviceProfileId_Timestamp",
                table: "SensorsData",
                columns: new[] { "DeviceProfileId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SensorsData_DeviceProfileId_Timestamp",
                table: "SensorsData");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "DeviceProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_SensorsData_DeviceProfileId",
                table: "SensorsData",
                column: "DeviceProfileId");
        }
    }
}
