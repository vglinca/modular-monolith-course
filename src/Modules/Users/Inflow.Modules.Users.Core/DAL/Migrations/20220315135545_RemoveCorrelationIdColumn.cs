using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inflow.Modules.Users.Core.DAL.Migrations
{
    public partial class RemoveCorrelationIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                schema: "users",
                table: "Outbox");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                schema: "users",
                table: "Outbox",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
