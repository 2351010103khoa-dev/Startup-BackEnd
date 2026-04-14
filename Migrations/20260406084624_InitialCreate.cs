using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StartupBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuongTrinhDaoTaos",
                columns: table => new
                {
                    MaCTDT = table.Column<string>(type: "text", nullable: false),
                    TenCTDT = table.Column<string>(type: "text", nullable: false),
                    TrinhDo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuongTrinhDaoTaos", x => x.MaCTDT);
                });

            migrationBuilder.CreateTable(
                name: "VaiTros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenVaiTro = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonHocs",
                columns: table => new
                {
                    MaMonHoc = table.Column<string>(type: "text", nullable: false),
                    TenMonHoc = table.Column<string>(type: "text", nullable: false),
                    SoTinChiLyThuyet = table.Column<int>(type: "integer", nullable: false),
                    SoTinChiThucHanh = table.Column<int>(type: "integer", nullable: false),
                    TrangThaiHoanThanh = table.Column<string>(type: "text", nullable: false),
                    ChuongTrinhDaoTaoMa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHocs", x => x.MaMonHoc);
                    table.ForeignKey(
                        name: "FK_MonHocs_ChuongTrinhDaoTaos_ChuongTrinhDaoTaoMa",
                        column: x => x.ChuongTrinhDaoTaoMa,
                        principalTable: "ChuongTrinhDaoTaos",
                        principalColumn: "MaCTDT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenDangNhap = table.Column<string>(type: "text", nullable: false),
                    MatKhau = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    HoTenNguoiDung = table.Column<string>(type: "text", nullable: false),
                    TrangThai = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    VaiTroId = table.Column<int>(type: "integer", nullable: false),
                    MaCTDT = table.Column<string>(type: "text", nullable: true),
                    HocHam = table.Column<string>(type: "text", nullable: true),
                    HocVi = table.Column<string>(type: "text", nullable: true),
                    TrinhDoChuyenMon = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiKhoans_ChuongTrinhDaoTaos_MaCTDT",
                        column: x => x.MaCTDT,
                        principalTable: "ChuongTrinhDaoTaos",
                        principalColumn: "MaCTDT");
                    table.ForeignKey(
                        name: "FK_TaiKhoans_VaiTros_VaiTroId",
                        column: x => x.VaiTroId,
                        principalTable: "VaiTros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongBienSoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NgayPhanCong = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NguoiBienSoanId = table.Column<int>(type: "integer", nullable: false),
                    MaMonHoc = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongBienSoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanCongBienSoans_MonHocs_MaMonHoc",
                        column: x => x.MaMonHoc,
                        principalTable: "MonHocs",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanCongBienSoans_TaiKhoans_NguoiBienSoanId",
                        column: x => x.NguoiBienSoanId,
                        principalTable: "TaiKhoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonHocs_ChuongTrinhDaoTaoMa",
                table: "MonHocs",
                column: "ChuongTrinhDaoTaoMa");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongBienSoans_MaMonHoc",
                table: "PhanCongBienSoans",
                column: "MaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongBienSoans_NguoiBienSoanId",
                table: "PhanCongBienSoans",
                column: "NguoiBienSoanId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_Email",
                table: "TaiKhoans",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_MaCTDT",
                table: "TaiKhoans",
                column: "MaCTDT");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_VaiTroId",
                table: "TaiKhoans",
                column: "VaiTroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhanCongBienSoans");

            migrationBuilder.DropTable(
                name: "MonHocs");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "ChuongTrinhDaoTaos");

            migrationBuilder.DropTable(
                name: "VaiTros");
        }
    }
}
