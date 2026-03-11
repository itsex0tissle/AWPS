using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWPS.IoT.Server.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Migration0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasuringDataSet",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    Light = table.Column<byte>(type: "tinyint", nullable: false),
                    Moisture = table.Column<byte>(type: "tinyint", nullable: false),
                    Humidity = table.Column<byte>(type: "tinyint", nullable: false),
                    Temperature = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuringDataSet", x => x.Guid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasuringDataSet");
        }
    }
}
