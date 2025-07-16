using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Merit_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class remove_unnecessary_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
