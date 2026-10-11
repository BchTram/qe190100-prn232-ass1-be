using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Migrations;

[DbContext(typeof(Prn232PostgresContext))]
[Migration("202610110001_AddSystemAccount")]
public partial class AddSystemAccount : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SystemAccount",
            columns: table => new
            {
                AccountID = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Role = table.Column<int>(type: "integer", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            },
            constraints: table =>
            {
                table.PrimaryKey("SystemAccount_pkey", row => row.AccountID);
            });

        migrationBuilder.AddColumn<int>(
            name: "CreatedByAccountID",
            table: "Task",
            type: "integer",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "SystemAccount_Email_key",
            table: "SystemAccount",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Task_CreatedByAccountID",
            table: "Task",
            column: "CreatedByAccountID");

        migrationBuilder.AddForeignKey(
            name: "FK_Task_SystemAccount",
            table: "Task",
            column: "CreatedByAccountID",
            principalTable: "SystemAccount",
            principalColumn: "AccountID",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Task_SystemAccount",
            table: "Task");

        migrationBuilder.DropIndex(
            name: "IX_Task_CreatedByAccountID",
            table: "Task");

        migrationBuilder.DropColumn(
            name: "CreatedByAccountID",
            table: "Task");

        migrationBuilder.DropTable(name: "SystemAccount");
    }
}