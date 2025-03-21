using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BashindaAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upazila = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Village = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanViewUserName = table.Column<bool>(type: "bit", nullable: false),
                    CanViewEmail = table.Column<bool>(type: "bit", nullable: false),
                    CanViewPhone = table.Column<bool>(type: "bit", nullable: false),
                    CanViewAddress = table.Column<bool>(type: "bit", nullable: false),
                    CanViewProfileImage = table.Column<bool>(type: "bit", nullable: false),
                    CanViewNationalId = table.Column<bool>(type: "bit", nullable: false),
                    CanViewBirthRegistration = table.Column<bool>(type: "bit", nullable: false),
                    CanViewDateOfBirth = table.Column<bool>(type: "bit", nullable: false),
                    CanViewFamilyInfo = table.Column<bool>(type: "bit", nullable: false),
                    CanViewProfession = table.Column<bool>(type: "bit", nullable: false),
                    CanApproveRenters = table.Column<bool>(type: "bit", nullable: false),
                    CanApproveOwners = table.Column<bool>(type: "bit", nullable: false),
                    CanManageApartments = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminPermissions_UserId",
                table: "AdminPermissions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminPermissions");
        }
    }
}
