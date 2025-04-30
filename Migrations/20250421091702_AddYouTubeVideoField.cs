using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMS.Migrations
{
    /// <inheritdoc />
    public partial class AddYouTubeVideoField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Hostels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Hostels");
        }
    }
}
