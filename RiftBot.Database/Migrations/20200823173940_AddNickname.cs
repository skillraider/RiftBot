using Microsoft.EntityFrameworkCore.Migrations;

namespace RiftBot.Database.Migrations
{
    public partial class AddNickname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "DiscordClanMember",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "DiscordClanMember");
        }
    }
}
