using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class AlterRefreshTokens : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "expired_at_utc",
            schema: "identity",
            table: "refresh_tokens",
            newName: "expires_at_utc");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "expires_at_utc",
            schema: "identity",
            table: "refresh_tokens",
            newName: "expired_at_utc");
    }
}
