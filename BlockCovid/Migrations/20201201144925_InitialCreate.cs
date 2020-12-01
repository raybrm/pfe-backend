using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlockCovid.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citizens",
                columns: table => new
                {
                    CitizenID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    First_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Last_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is_Positive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citizens", x => x.CitizenID);
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    ParticipantID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Participant_Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => x.ParticipantID);
                });

            migrationBuilder.CreateTable(
                name: "QrCode",
                columns: table => new
                {
                    QrCodeID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descritpion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCode", x => x.QrCodeID);
                    table.ForeignKey(
                        name: "FK_QrCode_Participant_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Participant",
                        principalColumn: "ParticipantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenQrCode",
                columns: table => new
                {
                    CitizenQrCodeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QrCodeId = table.Column<long>(type: "bigint", nullable: false),
                    CitizenId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenQrCode", x => x.CitizenQrCodeId);
                    table.ForeignKey(
                        name: "FK_CitizenQrCode_Citizens_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "Citizens",
                        principalColumn: "CitizenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenQrCode_QrCode_QrCodeId",
                        column: x => x.QrCodeId,
                        principalTable: "QrCode",
                        principalColumn: "QrCodeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenQrCode_CitizenId",
                table: "CitizenQrCode",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenQrCode_QrCodeId",
                table: "CitizenQrCode",
                column: "QrCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_QrCode_ParticipantID",
                table: "QrCode",
                column: "ParticipantID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenQrCode");

            migrationBuilder.DropTable(
                name: "Citizens");

            migrationBuilder.DropTable(
                name: "QrCode");

            migrationBuilder.DropTable(
                name: "Participant");
        }
    }
}
