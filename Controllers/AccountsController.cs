using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupBackend.Data;
using StartupBackend.DTOs;
using StartupBackend.Models;
using System.Security.Claims;

namespace StartupBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        // tạo tài khoản mới
        [HttpPost("create-accounts")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDTOs request)
        {
            var creatorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (creatorIdClaim == null) return Unauthorized(new { message = "Không xác định được người dùng!" });

            int creatorId = int.Parse(creatorIdClaim);

            var creator = await _context.TaiKhoans.FindAsync(creatorId);
            if (creator == null) return Unauthorized();

            if (_context.TaiKhoans.Any(u => u.TenDangNhap == request.Username))
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });

            if (_context.TaiKhoans.Any(u => u.Email == request.Email))
                return BadRequest(new { message = "Email đã tồn tại!" });

            var newAccount = new Accounts
            {
                TenDangNhap = request.Username,
                Email = request.Email,
                HoTenNguoiDung = request.FullName,
                VaiTroId = request.RoleId,
                MaKhoa = request.Khoa,
                TrangThai = "Hoạt động",
                MaCTDT = request.Programs,
                HocHam = request.HocHam,
                HocVi = request.HocVi,
                TrinhDoChuyenMon = request.TrinhDoChuyenMon,

                // mật khẩu mặc định
                MatKhau = BCrypt.Net.BCrypt.HashPassword("abc123"),

                TenantId = creator.TenantId, 
                NgayTao = DateTime.UtcNow,   
                NguoiTaoId = creator.Id
            };

            _context.TaiKhoans.Add(newAccount);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo tài khoản thành công!", accountId = newAccount.Id });
        }

        // lấy danh sách tài khoản
        [HttpGet]
        public async Task<IActionResult> GetAccounts(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 4,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value?.ToUpper();

            int.TryParse(currentUserIdString, out int currentUserId);

            int currentRoleId = currentUserRole switch
            {
                "ADMIN" => 1,
                "MANAGER" => 2,
                _ => 3 // COMPILER
            };
            var query = _context.TaiKhoans.Include(a => a.VaiTro).AsQueryable();

            query = query.Where(a => a.VaiTroId != 1);

            if (currentRoleId == 1)
            {
            }
            else
            {
                query = query.Where(a => a.VaiTroId > currentRoleId && a.NguoiTaoId == currentUserId);
            }

            // logic: nếu Frontend có truyền search thì mới filter, nếu ko thì show tất cả
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(a => a.TenDangNhap.ToLower().Contains(s) || a.HoTenNguoiDung.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.TrangThai == status);
            }

            // phân trang
            var totalItems = await query.CountAsync();
            var data = await query
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(a => new AccountResponse
                {
                    Id = a.Id,
                    Username = a.TenDangNhap,
                    FullName = a.HoTenNguoiDung,
                    Email = a.Email,
                    Role = a.VaiTro.TenVaiTro,
                    Programs = a.ChuongTrinhDaoTao.TenCTDT,
                    Khoa = a.Khoa.TenKhoa,
                    HocHam = a.HocHam,
                    HocVi = a.HocVi,
                    TrinhDoChuyenMon = a.TrinhDoChuyenMon,
                    Status = a.TrangThai
                })
                .ToListAsync();

            return Ok(new { total = totalItems, data });
        }
    }
}
