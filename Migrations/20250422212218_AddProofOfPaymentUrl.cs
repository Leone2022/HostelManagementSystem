using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMS.Migrations
{
    /// <inheritdoc />
    public partial class AddProofOfPaymentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_UserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_UserId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MaintenanceRequests");

            migrationBuilder.RenameColumn(
                name: "ResolutionNotes",
                table: "MaintenanceRequests",
                newName: "StaffNotes");

            migrationBuilder.RenameColumn(
                name: "ResolutionDate",
                table: "MaintenanceRequests",
                newName: "ResolvedAt");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "MaintenanceRequests",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "MaintenanceRequests",
                newName: "ResolvedById");

            migrationBuilder.AddColumn<string>(
                name: "ProofOfPaymentUrl",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "MaintenanceRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MaintenanceRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "MaintenanceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReportedById",
                table: "MaintenanceRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandlordId",
                table: "Hostels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardenId",
                table: "Hostels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_ReportedById",
                table: "MaintenanceRequests",
                column: "ReportedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_ReportedById",
                table: "MaintenanceRequests",
                column: "ReportedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_ReportedById",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_ReportedById",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ProofOfPaymentUrl",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "ReportedById",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "LandlordId",
                table: "Hostels");

            migrationBuilder.DropColumn(
                name: "WardenId",
                table: "Hostels");

            migrationBuilder.RenameColumn(
                name: "StaffNotes",
                table: "MaintenanceRequests",
                newName: "ResolutionNotes");

            migrationBuilder.RenameColumn(
                name: "ResolvedById",
                table: "MaintenanceRequests",
                newName: "AssignedTo");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "MaintenanceRequests",
                newName: "ResolutionDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "MaintenanceRequests",
                newName: "RequestDate");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "MaintenanceRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MaintenanceRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_UserId",
                table: "MaintenanceRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_AspNetUsers_UserId",
                table: "MaintenanceRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
