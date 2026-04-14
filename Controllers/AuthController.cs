using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StartupBackend.Data;
using StartupBackend.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StartupBackend.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // API đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.TaiKhoans
                .Include(u => u.VaiTro)
                .FirstOrDefaultAsync(u => u.TenDangNhap == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.MatKhau))
            {
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }

            if (user.TrangThai != "Hoạt động")
            {
                return Forbid("Tài khoản của bạn đang bị khóa!");
            }

            // Tạo Token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.TenDangNhap),
                    new Claim(ClaimTypes.Role, user.VaiTro.TenVaiTro)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            
            return Ok(new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                User = new UserInfo
                {
                    Role = user.VaiTro.TenVaiTro,
                    Name = user.HoTenNguoiDung
                }
            });
        }

        // đổi mật khẩu lần đầu
        [HttpPost("change-password-first")]
        public async Task<IActionResult> ChangePasswordFirst([FromBody] ChangePasswordFirstRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp!" });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized(); 

            var userId = int.Parse(userIdClaim);

            var user = await _context.TaiKhoans.FindAsync(userId);
            if (user == null) return NotFound();

            // băm mật khẩu mới trước khi lưu
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Mật khẩu đã được đổi, hãy đăng nhập lại." });
        }

        
        
    }
}
