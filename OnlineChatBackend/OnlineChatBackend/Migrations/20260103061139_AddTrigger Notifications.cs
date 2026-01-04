using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineChatBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggerNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION dialog_insert_notifications()
                RETURNS TRIGGER AS $$
                BEGIN
                    INSERT INTO ""Notifications""(""DialogId"", ""UserId"", ""NewNotifications"")
                    VALUES
                        (NEW.""Id"", NEW.""FirstUserId"", false),
                        (NEW.""Id"", NEW.""SecondUserId"", false);
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER trg_dialog_insert_notifications
                AFTER INSERT ON ""Dialogs""
                FOR EACH ROW
                EXECUTE FUNCTION dialog_insert_notifications();
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_dialog_insert_notifications ON ""Dialogs"";
                DROP FUNCTION IF EXISTS dialog_insert_notifications();
            ");
        }
    }
}
