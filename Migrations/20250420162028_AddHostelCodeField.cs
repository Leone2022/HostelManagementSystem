using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMS.Migrations
{
    /// <inheritdoc />
    public partial class AddHostelCodeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HostelCode",
                table: "Hostels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostelCode",
                table: "Hostels");
        }
    }
}
