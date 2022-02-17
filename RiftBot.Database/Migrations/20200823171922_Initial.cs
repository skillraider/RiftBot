using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RiftBot.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClanMember",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ClanRank = table.Column<string>(nullable: true),
                    ClanExperience = table.Column<long>(nullable: false),
                    Clan = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanMember", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClanRankRequirement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    ClanRank = table.Column<int>(nullable: false),
                    ClanExperience = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanRankRequirement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClanMemberExperience",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    ClanExperience = table.Column<long>(nullable: false),
                    ClanMemberId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanMemberExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClanMemberExperience_ClanMember_ClanMemberId",
                        column: x => x.ClanMemberId,
                        principalTable: "ClanMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordClanMember",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    ClanMemberId = table.Column<int>(nullable: false),
                    DiscordUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordClanMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordClanMember_ClanMember_ClanMemberId",
                        column: x => x.ClanMemberId,
                        principalTable: "ClanMember",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClanMemberExperience_ClanMemberId",
                table: "ClanMemberExperience",
                column: "ClanMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordClanMember_ClanMemberId",
                table: "DiscordClanMember",
                column: "ClanMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clan");

            migrationBuilder.DropTable(
                name: "ClanMemberExperience");

            migrationBuilder.DropTable(
                name: "ClanRankRequirement");

            migrationBuilder.DropTable(
                name: "DiscordClanMember");

            migrationBuilder.DropTable(
                name: "ClanMember");
        }
    }
}
