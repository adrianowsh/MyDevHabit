using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class AddOthersFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "frequency_frequency_type",
            schema: "my_dev_habit",
            table: "habits",
            newName: "status");

        migrationBuilder.AddColumn<int>(
            name: "frequency_type",
            schema: "my_dev_habit",
            table: "habits",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "frequency_type",
            schema: "my_dev_habit",
            table: "habits");

        migrationBuilder.RenameColumn(
            name: "status",
            schema: "my_dev_habit",
            table: "habits",
            newName: "frequency_frequency_type");
    }
}
