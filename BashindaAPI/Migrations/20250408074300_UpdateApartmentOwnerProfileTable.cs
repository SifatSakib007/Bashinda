using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BashindaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApartmentOwnerProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BirthRegistrationImagePath",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthRegistrationNo",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdult",
                table: "ApartmentOwnerProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthRegistrationImagePath",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "BirthRegistrationNo",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "IsAdult",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "ApartmentOwnerProfiles");
        }
    }
}
