using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdatenamepotoUrlcollom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PotoUrl",
                table: "Targets",
                newName: "potoUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "potoUrl",
                table: "Targets",
                newName: "PotoUrl");
        }
    }
}
