using Microsoft.EntityFrameworkCore.Migrations;

namespace middlerApp.API.Migrations
{
    public partial class IDPContextRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MUserRoles_MRole_RoleId",
                table: "MUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MRole",
                table: "MRole");

            migrationBuilder.RenameTable(
                name: "MRole",
                newName: "Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MUserRoles_Roles_RoleId",
                table: "MUserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MUserRoles_Roles_RoleId",
                table: "MUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "MRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MRole",
                table: "MRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MUserRoles_MRole_RoleId",
                table: "MUserRoles",
                column: "RoleId",
                principalTable: "MRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
