using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StartupBackend.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StartupBackend.DTOs;
using StartupBackend.Models;

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

                // mật khẩu mặc định
                MatKhau = BCrypt.Net.BCrypt.HashPassword("abc123"),

                TenantId = creator.TenantId, 
                NgayTao = DateTime.UtcNow,   
                NguoiTao = creator.TenDangNhap 
            };

            _context.TaiKhoans.Add(newAccount);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo tài khoản thành công!", accountId = newAccount.Id });
        }
    }
}
