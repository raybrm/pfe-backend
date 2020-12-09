using Microsoft.EntityFrameworkCore.Migrations;

namespace BlockCovid.Migrations
{
    public partial class add_field_citizen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is_Exposed",
                table: "Citizens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Exposed",
                table: "Citizens");
        }
    }
}
