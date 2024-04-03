using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppsProject5.Migrations
{
    /// <inheritdoc />
    public partial class ihatemigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseOfferedYears_Courses_CourseId",
                table: "CourseOfferedYears");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "CourseOfferedYears",
                type: "Varchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "Varchar(10)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CourseOfferedYears",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseOfferedYears",
                table: "CourseOfferedYears",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOfferedYears_Courses_CourseId",
                table: "CourseOfferedYears",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseOfferedYears_Courses_CourseId",
                table: "CourseOfferedYears");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseOfferedYears",
                table: "CourseOfferedYears");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CourseOfferedYears");

            migrationBuilder.UpdateData(
                table: "CourseOfferedYears",
                keyColumn: "CourseId",
                keyValue: null,
                column: "CourseId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "CourseOfferedYears",
                type: "Varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "Varchar(10)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOfferedYears_Courses_CourseId",
                table: "CourseOfferedYears",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
