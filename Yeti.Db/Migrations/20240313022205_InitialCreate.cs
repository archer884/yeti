using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yeti.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Fragments",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 13, 2, 22, 5, 4, DateTimeKind.Unspecified).AddTicks(6770), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 3, 13, 2, 22, 5, 4, DateTimeKind.Unspecified).AddTicks(6770), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Fragments",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 13, 2, 22, 5, 4, DateTimeKind.Unspecified).AddTicks(6770), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 3, 13, 2, 22, 5, 4, DateTimeKind.Unspecified).AddTicks(6770), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Manuscripts",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 13, 2, 22, 5, 4, DateTimeKind.Unspecified).AddTicks(6760), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 3, 13, 2, 22, 5, 4, DateTimeKind.Unspecified).AddTicks(6760), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Fragments",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 13, 2, 20, 4, 860, DateTimeKind.Unspecified).AddTicks(5190), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 3, 13, 2, 20, 4, 860, DateTimeKind.Unspecified).AddTicks(5190), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Fragments",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 13, 2, 20, 4, 860, DateTimeKind.Unspecified).AddTicks(5200), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 3, 13, 2, 20, 4, 860, DateTimeKind.Unspecified).AddTicks(5200), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Manuscripts",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Created", "Updated" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 3, 13, 2, 20, 4, 860, DateTimeKind.Unspecified).AddTicks(5190), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 3, 13, 2, 20, 4, 860, DateTimeKind.Unspecified).AddTicks(5190), new TimeSpan(0, 0, 0, 0, 0)) });
        }
    }
}
