using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeolocationAPI.Infrastructure.DB.MSSQLExpress.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Failed",
                table: "BatchProcesses");

            migrationBuilder.DropColumn(
                name: "Successful",
                table: "BatchProcesses");

            migrationBuilder.DropColumn(
                name: "TotalRecords",
                table: "BatchProcesses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Failed",
                table: "BatchProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Successful",
                table: "BatchProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRecords",
                table: "BatchProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
