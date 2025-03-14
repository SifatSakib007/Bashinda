using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BashindaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "RenterProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "ApartmentOwnerProfiles");
        }
    }
}
