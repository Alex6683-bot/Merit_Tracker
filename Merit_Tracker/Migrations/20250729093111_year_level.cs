using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Merit_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class year_level : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YearLevel",
                table: "Merits",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearLevel",
                table: "Merits");
        }
    }
}
