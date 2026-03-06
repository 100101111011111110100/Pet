using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datasource.Migrations
{
    /// <inheritdoc />
    public partial class RefreshAndKpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__games__fields_FieldId",
                table: "_games");

            migrationBuilder.DropPrimaryKey(
                name: "PK__users",
                table: "_users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK__games_GameUId",
                table: "_games");

            migrationBuilder.DropPrimaryKey(
                name: "PK__games",
                table: "_games");

            migrationBuilder.DropPrimaryKey(
                name: "PK__fields",
                table: "_fields");

            migrationBuilder.DropPrimaryKey(
                name: "PK__creds",
                table: "_creds");

            migrationBuilder.RenameTable(
                name: "_users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "_games",
                newName: "Games");

            migrationBuilder.RenameTable(
                name: "_fields",
                newName: "Fields");

            migrationBuilder.RenameTable(
                name: "_creds",
                newName: "Creds");

            migrationBuilder.RenameIndex(
                name: "IX__games_FieldId",
                table: "Games",
                newName: "IX_Games_FieldId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "Games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Games_GameUId",
                table: "Games",
                column: "GameUId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Games",
                table: "Games",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fields",
                table: "Fields",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Creds",
                table: "Creds",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Kpd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WinPercent = table.Column<double>(type: "double precision", nullable: false),
                    LosPercent = table.Column<double>(type: "double precision", nullable: false),
                    GamesCount = table.Column<int>(type: "integer", nullable: false),
                    WinCount = table.Column<int>(type: "integer", nullable: false),
                    LosCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kpd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Fields_FieldId",
                table: "Games",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Fields_FieldId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "Kpd");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Games_GameUId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Games",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fields",
                table: "Fields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Creds",
                table: "Creds");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "_users");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "_games");

            migrationBuilder.RenameTable(
                name: "Fields",
                newName: "_fields");

            migrationBuilder.RenameTable(
                name: "Creds",
                newName: "_creds");

            migrationBuilder.RenameIndex(
                name: "IX_Games_FieldId",
                table: "_games",
                newName: "IX__games_FieldId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__users",
                table: "_users",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK__games_GameUId",
                table: "_games",
                column: "GameUId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__games",
                table: "_games",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__fields",
                table: "_fields",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__creds",
                table: "_creds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__games__fields_FieldId",
                table: "_games",
                column: "FieldId",
                principalTable: "_fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
