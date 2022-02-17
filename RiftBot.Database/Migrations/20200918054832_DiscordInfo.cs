using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RiftBot.Database.Migrations
{
    public partial class DiscordInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordUser",
                columns: table => new
                {
                    UserId = table.Column<decimal>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    UserType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Guild",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MemberCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Channel",
                columns: table => new
                {
                    ChannelId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ChannelType = table.Column<string>(nullable: true),
                    GuildId = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channel", x => x.ChannelId);
                    table.ForeignKey(
                        name: "FK_Channel_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGuild",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<decimal>(nullable: false),
                    DiscordUserId = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGuild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGuild_DiscordUser_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGuild_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channel_GuildId",
                table: "Channel",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuild_DiscordUserId",
                table: "UserGuild",
                column: "DiscordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuild_GuildId",
                table: "UserGuild",
                column: "GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channel");

            migrationBuilder.DropTable(
                name: "UserGuild");

            migrationBuilder.DropTable(
                name: "DiscordUser");

            migrationBuilder.DropTable(
                name: "Guild");
        }
    }
}
