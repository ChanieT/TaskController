using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Enum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "InProgress",
                table: "Assignments");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Assignments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Assignments");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Assignments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InProgress",
                table: "Assignments",
                nullable: false,
                defaultValue: false);
        }
    }
}
