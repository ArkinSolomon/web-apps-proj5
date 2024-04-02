using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppsProject5.Migrations
{
    /// <inheritdoc />
    public partial class idkwhatsgoingonanymore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Requirements",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "Varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requirements",
                table: "Requirements",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Requirements",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Requirements");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "Varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
