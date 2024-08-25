using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class addCollomPhotoUrlToAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "potoUrl",
                table: "Targets",
                newName: "photoUrl");

            migrationBuilder.RenameColumn(
                name: "NickName",
                table: "Agents",
                newName: "nickname");

            migrationBuilder.AddColumn<string>(
                name: "photoUrl",
                table: "Agents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photoUrl",
                table: "Agents");

            migrationBuilder.RenameColumn(
                name: "photoUrl",
                table: "Targets",
                newName: "potoUrl");

            migrationBuilder.RenameColumn(
                name: "nickname",
                table: "Agents",
                newName: "NickName");
        }
    }
}
