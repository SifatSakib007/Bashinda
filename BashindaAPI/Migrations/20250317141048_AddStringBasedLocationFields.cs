using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BashindaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStringBasedLocationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentOwnerProfiles_Districts_DistrictId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentOwnerProfiles_Divisions_DivisionId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentOwnerProfiles_Upazilas_UpazilaId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentOwnerProfiles_Villages_VillageId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentOwnerProfiles_Wards_WardId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RenterProfiles_Districts_DistrictId",
                table: "RenterProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RenterProfiles_Divisions_DivisionId",
                table: "RenterProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RenterProfiles_Upazilas_UpazilaId",
                table: "RenterProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RenterProfiles_Villages_VillageId",
                table: "RenterProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RenterProfiles_Wards_WardId",
                table: "RenterProfiles");

            migrationBuilder.DropIndex(
                name: "IX_RenterProfiles_DistrictId",
                table: "RenterProfiles");

            migrationBuilder.DropIndex(
                name: "IX_RenterProfiles_DivisionId",
                table: "RenterProfiles");

            migrationBuilder.DropIndex(
                name: "IX_RenterProfiles_UpazilaId",
                table: "RenterProfiles");

            migrationBuilder.DropIndex(
                name: "IX_RenterProfiles_VillageId",
                table: "RenterProfiles");

            migrationBuilder.DropIndex(
                name: "IX_RenterProfiles_WardId",
                table: "RenterProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentOwnerProfiles_DistrictId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentOwnerProfiles_DivisionId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentOwnerProfiles_UpazilaId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentOwnerProfiles_VillageId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ApartmentOwnerProfiles_WardId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "UpazilaId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "VillageId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "UpazilaId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "VillageId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "RenterProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "RenterProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Upazila",
                table: "RenterProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Village",
                table: "RenterProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "RenterProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Upazila",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Village",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "Upazila",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "Village",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "District",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "Upazila",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "Village",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "RenterProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "RenterProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpazilaId",
                table: "RenterProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VillageId",
                table: "RenterProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "RenterProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "ApartmentOwnerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "ApartmentOwnerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpazilaId",
                table: "ApartmentOwnerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VillageId",
                table: "ApartmentOwnerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "ApartmentOwnerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RenterProfiles_DistrictId",
                table: "RenterProfiles",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_RenterProfiles_DivisionId",
                table: "RenterProfiles",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_RenterProfiles_UpazilaId",
                table: "RenterProfiles",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_RenterProfiles_VillageId",
                table: "RenterProfiles",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_RenterProfiles_WardId",
                table: "RenterProfiles",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentOwnerProfiles_DistrictId",
                table: "ApartmentOwnerProfiles",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentOwnerProfiles_DivisionId",
                table: "ApartmentOwnerProfiles",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentOwnerProfiles_UpazilaId",
                table: "ApartmentOwnerProfiles",
                column: "UpazilaId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentOwnerProfiles_VillageId",
                table: "ApartmentOwnerProfiles",
                column: "VillageId");

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentOwnerProfiles_WardId",
                table: "ApartmentOwnerProfiles",
                column: "WardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Districts_DistrictId",
                table: "ApartmentOwnerProfiles",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Divisions_DivisionId",
                table: "ApartmentOwnerProfiles",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Upazilas_UpazilaId",
                table: "ApartmentOwnerProfiles",
                column: "UpazilaId",
                principalTable: "Upazilas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Villages_VillageId",
                table: "ApartmentOwnerProfiles",
                column: "VillageId",
                principalTable: "Villages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Wards_WardId",
                table: "ApartmentOwnerProfiles",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Districts_DistrictId",
                table: "RenterProfiles",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Divisions_DivisionId",
                table: "RenterProfiles",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Upazilas_UpazilaId",
                table: "RenterProfiles",
                column: "UpazilaId",
                principalTable: "Upazilas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Villages_VillageId",
                table: "RenterProfiles",
                column: "VillageId",
                principalTable: "Villages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Wards_WardId",
                table: "RenterProfiles",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");
        }
    }
}
