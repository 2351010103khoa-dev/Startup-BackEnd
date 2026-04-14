using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartupBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTableKhoas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaKhoa",
                table: "TaiKhoans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "TaiKhoans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NguoiTao",
                table: "TaiKhoans",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Khoas",
                columns: table => new
                {
                    MaKhoa = table.Column<string>(type: "text", nullable: false),
                    TenKhoa = table.Column<string>(type: "text", nullable: false),
                    MaCTDT = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khoas", x => x.MaKhoa);
                    table.ForeignKey(
                        name: "FK_Khoas_ChuongTrinhDaoTaos_MaCTDT",
                        column: x => x.MaCTDT,
                        principalTable: "ChuongTrinhDaoTaos",
                        principalColumn: "MaCTDT");
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaKhoa", "MatKhau", "NgayTao", "NguoiTao" },
                values: new object[] { null, "$2a$11$ysYS/qhQhg1fZEn/OZZboODtCMocdf.EyA5jiSiBUJlzS.MQOJhcS", new DateTime(2026, 4, 6, 10, 40, 42, 401, DateTimeKind.Utc).AddTicks(8647), null });

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_MaKhoa",
                table: "TaiKhoans",
                column: "MaKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_Khoas_MaCTDT",
                table: "Khoas",
                column: "MaCTDT");

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoans_Khoas_MaKhoa",
                table: "TaiKhoans",
                column: "MaKhoa",
                principalTable: "Khoas",
                principalColumn: "MaKhoa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoans_Khoas_MaKhoa",
                table: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "Khoas");

            migrationBuilder.DropIndex(
                name: "IX_TaiKhoans_MaKhoa",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "MaKhoa",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "NguoiTao",
                table: "TaiKhoans");

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "Id",
                keyValue: 1,
                column: "MatKhau",
                value: "$2a$11$JtCE3Ax4BPUDME0PW4wlZOSZjoW1pzstgAeAjfHQbku.Pv7m5N/bG");
        }
    }
}
