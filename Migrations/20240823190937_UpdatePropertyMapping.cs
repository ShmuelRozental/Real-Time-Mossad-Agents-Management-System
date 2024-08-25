using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Location_X",
                table: "Agents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Location_Y",
                table: "Agents",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_X",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Location_Y",
                table: "Agents");
        }
    }
}
