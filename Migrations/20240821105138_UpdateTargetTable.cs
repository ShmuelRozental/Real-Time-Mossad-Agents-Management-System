using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTargetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Targets",
                newName: "PotoUrl");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Targets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Targets");

            migrationBuilder.RenameColumn(
                name: "PotoUrl",
                table: "Targets",
                newName: "Description");
        }
    }
}
