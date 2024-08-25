using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Real_Time_Mossad_Agents_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddNickNameToAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NikName",
                table: "Agents",
                newName: "NickName");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetId",
                table: "Missions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Missions_TargetId",
                table: "Missions",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Targets_TargetId",
                table: "Missions",
                column: "TargetId",
                principalTable: "Targets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Targets_TargetId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_TargetId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Missions");

            migrationBuilder.RenameColumn(
                name: "NickName",
                table: "Agents",
                newName: "NikName");
        }
    }
}
