using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StartupBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VaiTros",
                columns: new[] { "Id", "TenVaiTro" },
                values: new object[,]
                {
                    { 1, "ADMIN" },
                    { 2, "MANAGER" },
                    { 3, "COMPILER" }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoans",
                columns: new[] { "Id", "Email", "HoTenNguoiDung", "HocHam", "HocVi", "MaCTDT", "MatKhau", "TenDangNhap", "TenantId", "TrangThai", "TrinhDoChuyenMon", "VaiTroId" },
                values: new object[] { 1, "admin@system.com", "Root Admin", null, null, null, "$2a$11$JtCE3Ax4BPUDME0PW4wlZOSZjoW1pzstgAeAjfHQbku.Pv7m5N/bG", "admin", "HCMCOU", "Hoạt động", null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaiKhoans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VaiTros",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VaiTros",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "VaiTros",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
