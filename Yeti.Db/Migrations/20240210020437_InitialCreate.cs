using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Yeti.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Writers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    AuthorName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Writers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manuscripts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WriterId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Blurb = table.Column<string>(type: "text", nullable: true),
                    SoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manuscripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manuscripts_Writers_WriterId",
                        column: x => x.WriterId,
                        principalTable: "Writers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fragments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WriterId = table.Column<long>(type: "bigint", nullable: false),
                    ManuscriptId = table.Column<long>(type: "bigint", nullable: false),
                    Heading = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SortBy = table.Column<double>(type: "double precision", nullable: false),
                    SoftDelete = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fragments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fragments_Manuscripts_ManuscriptId",
                        column: x => x.ManuscriptId,
                        principalTable: "Manuscripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fragments_Writers_WriterId",
                        column: x => x.WriterId,
                        principalTable: "Writers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManuscriptTag",
                columns: table => new
                {
                    ManuscriptsId = table.Column<long>(type: "bigint", nullable: false),
                    TagsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManuscriptTag", x => new { x.ManuscriptsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ManuscriptTag_Manuscripts_ManuscriptsId",
                        column: x => x.ManuscriptsId,
                        principalTable: "Manuscripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManuscriptTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Writers",
                columns: new[] { "Id", "AuthorName", "Username" },
                values: new object[] { 1L, "H. Wadsworth Longfellow", "longfellow" });

            migrationBuilder.InsertData(
                table: "Manuscripts",
                columns: new[] { "Id", "Blurb", "Created", "SoftDelete", "Title", "Updated", "WriterId" },
                values: new object[] { 1L, "An old poem", new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2510), new TimeSpan(0, 0, 0, 0, 0)), false, "The Song of Hiawatha", new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2510), new TimeSpan(0, 0, 0, 0, 0)), 1L });

            migrationBuilder.InsertData(
                table: "Fragments",
                columns: new[] { "Id", "Content", "Created", "Heading", "ManuscriptId", "SoftDelete", "SortBy", "Updated", "WriterId" },
                values: new object[,]
                {
                    { 1L, "By the shore of Gitche Gumbee,\nBy the shining Big-Sea-Water,\nAt the doorway of his wigwam,\nIn the pleasant Summer morning,\nHiawatha stood and waited.", new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2510), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, true, 1.0, new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2520), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 2L, "By the shore of Gitche Gumee,\nBy the shining Big-Sea-Water,\nAt the doorway of his wigwam,\nIn the pleasant Summer morning,\nHiawatha stood and waited.", new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2520), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, false, 1.0, new DateTimeOffset(new DateTime(2024, 2, 10, 2, 4, 37, 479, DateTimeKind.Unspecified).AddTicks(2520), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fragments_Created",
                table: "Fragments",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Fragments_ManuscriptId",
                table: "Fragments",
                column: "ManuscriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Fragments_SoftDelete",
                table: "Fragments",
                column: "SoftDelete");

            migrationBuilder.CreateIndex(
                name: "IX_Fragments_WriterId",
                table: "Fragments",
                column: "WriterId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_Created",
                table: "Manuscripts",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_SoftDelete",
                table: "Manuscripts",
                column: "SoftDelete");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_Title",
                table: "Manuscripts",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_WriterId",
                table: "Manuscripts",
                column: "WriterId");

            migrationBuilder.CreateIndex(
                name: "IX_ManuscriptTag_TagsId",
                table: "ManuscriptTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                column: "Value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fragments");

            migrationBuilder.DropTable(
                name: "ManuscriptTag");

            migrationBuilder.DropTable(
                name: "Manuscripts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Writers");
        }
    }
}
