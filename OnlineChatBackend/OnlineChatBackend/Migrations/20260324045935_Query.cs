using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OnlineChatBackend.Migrations
{
    /// <inheritdoc />
    public partial class Query : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSearchResult",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SearchQueryId",
                table: "Messages",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SearchQueries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ChatId = table.Column<int>(type: "integer", nullable: false),
                    QueryText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchQueries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchQueries_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SearchQueries_Contacts_UserId",
                        column: x => x.UserId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SearchQueryId",
                table: "Messages",
                column: "SearchQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchQueries_ChatId",
                table: "SearchQueries",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchQueries_UserId",
                table: "SearchQueries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SearchQueries_SearchQueryId",
                table: "Messages",
                column: "SearchQueryId",
                principalTable: "SearchQueries",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SearchQueries_SearchQueryId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "SearchQueries");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SearchQueryId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsSearchResult",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SearchQueryId",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
