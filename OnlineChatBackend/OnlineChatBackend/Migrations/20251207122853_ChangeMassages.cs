using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineChatBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMassages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Messages",
                newName: "ToUserId");

            migrationBuilder.AddColumn<int>(
                name: "FromUserId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Messages",
                newName: "UserId");
        }
    }
}
