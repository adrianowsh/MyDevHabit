﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class AddTagTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "tags",
            schema: "my_dev_habit",
            columns: table => new
            {
                id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                update_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_tags", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_tags_name",
            schema: "my_dev_habit",
            table: "tags",
            column: "name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "tags",
            schema: "my_dev_habit");
    }
}
