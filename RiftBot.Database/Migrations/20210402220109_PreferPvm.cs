using Microsoft.EntityFrameworkCore.Migrations;

namespace RiftBot.Database.Migrations
{
    public partial class PreferPvm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PreferPvm",
                table: "ClanMember",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferPvm",
                table: "ClanMember");
        }
    }
}
