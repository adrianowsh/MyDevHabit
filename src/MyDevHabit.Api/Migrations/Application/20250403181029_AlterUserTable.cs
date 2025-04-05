using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class AlterUserTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_user",
            schema: "my_dev_habit",
            table: "user");

        migrationBuilder.RenameTable(
            name: "user",
            schema: "my_dev_habit",
            newName: "users",
            newSchema: "my_dev_habit");

        migrationBuilder.RenameIndex(
            name: "ix_user_identity_id",
            schema: "my_dev_habit",
            table: "users",
            newName: "ix_users_identity_id");

        migrationBuilder.RenameIndex(
            name: "ix_user_email",
            schema: "my_dev_habit",
            table: "users",
            newName: "ix_users_email");

        migrationBuilder.AddPrimaryKey(
            name: "pk_users",
            schema: "my_dev_habit",
            table: "users",
            column: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "pk_users",
            schema: "my_dev_habit",
            table: "users");

        migrationBuilder.RenameTable(
            name: "users",
            schema: "my_dev_habit",
            newName: "user",
            newSchema: "my_dev_habit");

        migrationBuilder.RenameIndex(
            name: "ix_users_identity_id",
            schema: "my_dev_habit",
            table: "user",
            newName: "ix_user_identity_id");

        migrationBuilder.RenameIndex(
            name: "ix_users_email",
            schema: "my_dev_habit",
            table: "user",
            newName: "ix_user_email");

        migrationBuilder.AddPrimaryKey(
            name: "pk_user",
            schema: "my_dev_habit",
            table: "user",
            column: "id");
    }
}
