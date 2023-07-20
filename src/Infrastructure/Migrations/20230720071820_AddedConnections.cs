using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ConnectionId",
                table: "Users",
                column: "ConnectionId",
                unique: true,
                filter: "[ConnectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_SessionId",
                table: "Connections",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Connections_ConnectionId",
                table: "Users",
                column: "ConnectionId",
                principalTable: "Connections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Connections_ConnectionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropIndex(
                name: "IX_Users_ConnectionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Users");
        }
    }
}
