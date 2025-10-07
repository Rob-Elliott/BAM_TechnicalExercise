using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CareerStartDate",
                table: "AstronautDetail",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "John Doe" },
                    { 2, "Jane Doe" },
                    { 3, "John Young" },
                    { 4, "Michael Collins" },
                    { 5, "Virgil Grissom" }
                });

            migrationBuilder.InsertData(
                table: "AstronautDetail",
                columns: new[] { "Id", "CareerEndDate", "CareerStartDate", "CurrentDutyTitle", "CurrentRank", "PersonId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2020, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Commander", "1LT", 1 },
                    { 2, null, new DateTime(1969, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Commander", "LCL", 3 },
                    { 3, new DateTime(1969, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1967, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "RETIRED", "CPT", 4 },
                    { 4, new DateTime(1966, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "RETIRED", "1LT", 5 }
                });

            migrationBuilder.InsertData(
                table: "AstronautDuty",
                columns: new[] { "Id", "DutyEndDate", "DutyStartDate", "DutyTitle", "PersonId", "Rank" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Commander", 1, "1LT" },
                    { 2, new DateTime(1966, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1962, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pilot", 3, "1LT" },
                    { 3, new DateTime(1967, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1966, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Command Pilot", 3, "CPT" },
                    { 4, new DateTime(1969, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1967, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Command Module Pilot", 3, "MAJ" },
                    { 5, new DateTime(1983, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1969, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Commander", 3, "LCL" },
                    { 6, new DateTime(1967, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1962, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pilot", 4, "1LT" },
                    { 7, new DateTime(1969, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1967, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Command Module Pilot", 4, "CPT" },
                    { 8, new DateTime(1966, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1960, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Command Pilot", 5, "1LT" },
                    { 9, null, new DateTime(1969, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "RETIRED", 4, "CPT" },
                    { 10, null, new DateTime(1966, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "RETIRED", 5, "1LT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AstronautDuty",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CareerStartDate",
                table: "AstronautDetail",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }
    }
}
