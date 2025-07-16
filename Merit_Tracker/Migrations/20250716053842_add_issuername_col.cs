using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Merit_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class add_issuername_col : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IssueName",
                table: "Merits",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueName",
                table: "Merits");
        }
    }
}
