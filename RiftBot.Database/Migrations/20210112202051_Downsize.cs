using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RiftBot.Database.Migrations
{
    public partial class Downsize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channel");

            migrationBuilder.DropTable(
                name: "ClanMemberNameChange");

            migrationBuilder.DropTable(
                name: "DiscordMember");

            migrationBuilder.DropTable(
                name: "UserGuild");

            migrationBuilder.DropTable(
                name: "DiscordUser");

            migrationBuilder.DropTable(
                name: "Guild");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClanMemberNameChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentName = table.Column<string>(type: "text", nullable: false),
                    PreviousName = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanMemberNameChange", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscordMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordId = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Timezone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordMember_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordUser",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Guild",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    MemberCount = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "Channel",
                columns: table => new
                {
                    ChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ChannelType = table.Column<string>(type: "text", nullable: true),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordUserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
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
                name: "IX_DiscordMember_PlayerId",
                table: "DiscordMember",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuild_DiscordUserId",
                table: "UserGuild",
                column: "DiscordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuild_GuildId",
                table: "UserGuild",
                column: "GuildId");
        }
    }
}
