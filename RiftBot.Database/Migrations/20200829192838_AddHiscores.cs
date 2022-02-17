using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RiftBot.Types;

namespace RiftBot.Database.Migrations
{
    public partial class AddHiscores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordClanMember");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "ClanRankRequirement");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClanRankRequirement");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ClanRankRequirement");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "ClanMemberExperience");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClanMemberExperience");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ClanMemberExperience");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "Clan",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "Clan");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Clan");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Clan");

            migrationBuilder.AddColumn<int>(
                name: "ClanId",
                table: "ClanRankRequirement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ClanRank",
                table: "ClanMember",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClanId",
                table: "ClanMember",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasLeftClan",
                table: "ClanMember",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "ClanMember",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Clan",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Clan",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfMembers",
                table: "Clan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartedTracking",
                table: "Clan",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "ClanMemberNameChange",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreviousName = table.Column<string>(nullable: false),
                    CurrentName = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanMemberNameChange", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    IsIronman = table.Column<bool>(nullable: false),
                    IsHardcore = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscordMember",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiscordId = table.Column<string>(nullable: false),
                    Timezone = table.Column<string>(nullable: true),
                    PlayerId = table.Column<int>(nullable: false)
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
                name: "PlayerExperience",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(nullable: false),
                    SkillStats = table.Column<SkillStats[]>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerExperience_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClanRankRequirement_ClanId",
                table: "ClanRankRequirement",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_ClanMember_ClanId",
                table: "ClanMember",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordMember_PlayerId",
                table: "DiscordMember",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerExperience_PlayerId",
                table: "PlayerExperience",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClanMember_Clan_ClanId",
                table: "ClanMember",
                column: "ClanId",
                principalTable: "Clan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClanRankRequirement_Clan_ClanId",
                table: "ClanRankRequirement",
                column: "ClanId",
                principalTable: "Clan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClanMember_Clan_ClanId",
                table: "ClanMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ClanRankRequirement_Clan_ClanId",
                table: "ClanRankRequirement");

            migrationBuilder.DropTable(
                name: "ClanMemberNameChange");

            migrationBuilder.DropTable(
                name: "DiscordMember");

            migrationBuilder.DropTable(
                name: "PlayerExperience");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropIndex(
                name: "IX_ClanRankRequirement_ClanId",
                table: "ClanRankRequirement");

            migrationBuilder.DropIndex(
                name: "IX_ClanMember_ClanId",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "ClanId",
                table: "ClanRankRequirement");

            migrationBuilder.DropColumn(
                name: "ClanId",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "HasLeftClan",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "ClanMember");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Clan");

            migrationBuilder.DropColumn(
                name: "NumberOfMembers",
                table: "Clan");

            migrationBuilder.DropColumn(
                name: "StartedTracking",
                table: "Clan");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "ClanRankRequirement",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClanRankRequirement",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ClanRankRequirement",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "ClanMemberExperience",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClanMemberExperience",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ClanMemberExperience",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClanRank",
                table: "ClanMember",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "ClanMember",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Clan",
                table: "ClanMember",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClanMember",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ClanMember",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Clan",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "Clan",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Clan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Clan",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiscordClanMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ClanMemberId = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DiscordUserId = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: true)
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
                name: "IX_DiscordClanMember_ClanMemberId",
                table: "DiscordClanMember",
                column: "ClanMemberId");
        }
    }
}
