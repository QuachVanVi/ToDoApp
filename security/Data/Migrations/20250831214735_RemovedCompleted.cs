using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace security.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Todos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Todos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
