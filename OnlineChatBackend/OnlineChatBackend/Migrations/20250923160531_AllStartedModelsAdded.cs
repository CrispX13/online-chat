using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OnlineChatBackend.Migrations
{
    /// <inheritdoc />
    public partial class AllStartedModelsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DialogKey",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Contacts");

            migrationBuilder.AddColumn<int>(
                name: "DialogId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Dialogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FirstUserId = table.Column<int>(type: "integer", nullable: false),
                    SecondUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dialogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dialogs_Contacts_FirstUserId",
                        column: x => x.FirstUserId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dialogs_Contacts_SecondUserId",
                        column: x => x.SecondUserId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DialogId",
                table: "Messages",
                column: "DialogId");

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_FirstUserId_SecondUserId",
                table: "Dialogs",
                columns: new[] { "FirstUserId", "SecondUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dialogs_SecondUserId",
                table: "Dialogs",
                column: "SecondUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Dialogs_DialogId",
                table: "Messages",
                column: "DialogId",
                principalTable: "Dialogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Dialogs_DialogId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Dialogs");

            migrationBuilder.DropIndex(
                name: "IX_Messages_DialogId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DialogId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "DialogKey",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
