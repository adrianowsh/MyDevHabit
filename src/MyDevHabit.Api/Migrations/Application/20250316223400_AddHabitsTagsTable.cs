using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class AddHabitsTagsTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "update_at_utc",
            schema: "my_dev_habit",
            table: "tags");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_at_utc",
            schema: "my_dev_habit",
            table: "tags",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "habits_tags",
            schema: "my_dev_habit",
            columns: table => new
            {
                habit_id = table.Column<string>(type: "character varying(500)", nullable: false),
                tag_id = table.Column<string>(type: "character varying(500)", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_habits_tags", x => new { x.habit_id, x.tag_id });
                table.ForeignKey(
                    name: "fk_habits_tags_habits_habit_id",
                    column: x => x.habit_id,
                    principalSchema: "my_dev_habit",
                    principalTable: "habits",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_habits_tags_tags_tag_id",
                    column: x => x.tag_id,
                    principalSchema: "my_dev_habit",
                    principalTable: "tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_habits_tags_tag_id",
            schema: "my_dev_habit",
            table: "habits_tags",
            column: "tag_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "habits_tags",
            schema: "my_dev_habit");

        migrationBuilder.DropColumn(
            name: "updated_at_utc",
            schema: "my_dev_habit",
            table: "tags");

        migrationBuilder.AddColumn<DateTime>(
            name: "update_at_utc",
            schema: "my_dev_habit",
            table: "tags",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }
}
