using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMS.Migrations
{
    /// <inheritdoc />
    public partial class AddHostelManagementType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagementType",
                table: "Hostels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagementType",
                table: "Hostels");
        }
    }
}
