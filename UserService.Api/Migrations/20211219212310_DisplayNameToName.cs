using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Migrations
{
    public partial class DisplayNameToName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderName",
                table: "AspNetUserLogins",
                newName: "ProviderDisplayName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                newName: "ProviderName");
        }
    }
}
