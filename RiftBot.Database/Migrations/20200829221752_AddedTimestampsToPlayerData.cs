using System;
using Microsoft.EntityFrameworkCore.Migrations;
using RiftBot.Types;

namespace RiftBot.Database.Migrations
{
    public partial class AddedTimestampsToPlayerData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<SkillStats[]>(
                name: "SkillStats",
                table: "PlayerExperience",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(SkillStats[]),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "PlayerExperience",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Player",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartedTracking",
                table: "Player",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "PlayerExperience");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "StartedTracking",
                table: "Player");

            migrationBuilder.AlterColumn<SkillStats[]>(
                name: "SkillStats",
                table: "PlayerExperience",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(SkillStats[]),
                oldType: "jsonb");
        }
    }
}
