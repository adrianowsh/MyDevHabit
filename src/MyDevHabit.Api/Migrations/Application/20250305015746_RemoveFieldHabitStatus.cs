using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class RemoveFieldHabitStatus : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "habit_status",
            schema: "my_dev_habit",
            table: "habits");

        migrationBuilder.AlterColumn<int>(
            name: "status",
            schema: "my_dev_habit",
            table: "habits",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "status",
            schema: "my_dev_habit",
            table: "habits",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AddColumn<int>(
            name: "habit_status",
            schema: "my_dev_habit",
            table: "habits",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }
}
