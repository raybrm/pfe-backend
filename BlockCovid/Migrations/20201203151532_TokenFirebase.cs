using Microsoft.EntityFrameworkCore.Migrations;

namespace BlockCovid.Migrations
{
    public partial class TokenFirebase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenFireBase",
                table: "Citizens",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenFireBase",
                table: "Citizens");
        }
    }
}
