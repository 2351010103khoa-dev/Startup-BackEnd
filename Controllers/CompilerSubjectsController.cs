using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupBackend.Data;
using StartupBackend.DTOs;

namespace StartupBackend.Controllers
{
    [Route("compiler")]
    [ApiController]
    [Authorize]
    public class CompilerSubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompilerSubjectsController(AppDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // NHÓM CHỨC NĂNG: BIÊN SOẠN ĐỀ CƯƠNG (Dành cho Compiler)
        // ==============================================================================

        // 1. Lấy danh sách môn học được phân công
        // Endpoint: GET /compiler/subjects
        [HttpGet("subjects")]
        public async Task<IActionResult> GetAssignedSubjects()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin đăng nhập" });

            int currentUserId = int.Parse(userIdClaim.Value);

            var assignedSubjects = await _context.MonHocs
                .Where(s => s.PhanCongBienSoans.Any(pc => pc.NguoiBienSoanId == currentUserId))
                .Select(s => new CompilerSubjectResponse
                {
                    MaMonHoc = s.MaMonHoc,
                    TenMonHoc = s.TenMonHoc,
                    SoTinChi = s.SoTinChiLyThuyet + s.SoTinChiThucHanh,
                    TrangThaiHoanThanh = s.TrangThaiHoanThanh,
                    TenChuongTrinh = s.ChuongTrinhDaoTao.TenCTDT
                })
                .ToListAsync();

            return Ok(new { data = assignedSubjects });
        }
    }
}