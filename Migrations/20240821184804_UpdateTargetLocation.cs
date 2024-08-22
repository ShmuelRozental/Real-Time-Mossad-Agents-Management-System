using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTargetLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Location_X",
                table: "Targets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Location_Y",
                table: "Targets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_X",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "Location_Y",
                table: "Targets");
        }
    }
}
