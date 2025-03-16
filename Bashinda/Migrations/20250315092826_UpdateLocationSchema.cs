using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bashinda.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLocationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentOwnerProfiles_IdentityUser_UserId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.DropColumn(
                name: "District",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.RenameColumn(
                name: "Upazila",
                table: "RenterProfiles",
                newName: "HoldingNo");

            migrationBuilder.AddColumn<int>(
                name: "AreaType",
                table: "RenterProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "RenterProfiles",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "RenterProfiles",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ApartmentOwnerProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Profession",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AreaType",
                table: "ApartmentOwnerProfiles",
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

            migrationBuilder.AddColumn<string>(
                name: "HoldingNo",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DivisionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Upazilas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upazilas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Upazilas_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpazilaId = table.Column<int>(type: "int", nullable: false),
                    AreaType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Upazilas_UpazilaId",
                        column: x => x.UpazilaId,
                        principalTable: "Upazilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Villages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Villages_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Districts_DivisionId",
                table: "Districts",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Upazilas_DistrictId",
                table: "Upazilas",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Villages_WardId",
                table: "Villages",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_UpazilaId",
                table: "Wards",
                column: "UpazilaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Districts_DistrictId",
                table: "ApartmentOwnerProfiles",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Divisions_DivisionId",
                table: "ApartmentOwnerProfiles",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Upazilas_UpazilaId",
                table: "ApartmentOwnerProfiles",
                column: "UpazilaId",
                principalTable: "Upazilas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Users_UserId",
                table: "ApartmentOwnerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Villages_VillageId",
                table: "ApartmentOwnerProfiles",
                column: "VillageId",
                principalTable: "Villages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_Wards_WardId",
                table: "ApartmentOwnerProfiles",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Districts_DistrictId",
                table: "RenterProfiles",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Divisions_DivisionId",
                table: "RenterProfiles",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Upazilas_UpazilaId",
                table: "RenterProfiles",
                column: "UpazilaId",
                principalTable: "Upazilas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Villages_VillageId",
                table: "RenterProfiles",
                column: "VillageId",
                principalTable: "Villages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RenterProfiles_Wards_WardId",
                table: "RenterProfiles",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_ApartmentOwnerProfiles_Users_UserId",
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

            migrationBuilder.DropTable(
                name: "Villages");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Upazilas");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Divisions");

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
                name: "AreaType",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
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
                name: "AreaType",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "HoldingNo",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "ApartmentOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
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

            migrationBuilder.RenameColumn(
                name: "HoldingNo",
                table: "RenterProfiles",
                newName: "Upazila");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "RenterProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "RenterProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Profession",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ApartmentOwnerProfiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentOwnerProfiles_IdentityUser_UserId",
                table: "ApartmentOwnerProfiles",
                column: "UserId",
                principalTable: "IdentityUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
