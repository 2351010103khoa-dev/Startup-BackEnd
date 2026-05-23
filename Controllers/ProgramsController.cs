using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupBackend.Data;
using StartupBackend.DTOs;
using StartupBackend.Models;

namespace StartupBackend.Controllers
{
    [Route("api/programs")]
    [ApiController]
    [Authorize] 
    public class ProgramsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramsController(AppDbContext context)
        {
            _context = context;
        }

// api thêm chương trình đào tạo mới
        [HttpPost]
        public async Task<IActionResult> CreateProgram([FromBody] ProgramsDTOs request)
        {
            // check mã trùng
            if (await _context.ChuongTrinhDaoTaos.AnyAsync(p => p.MaCTDT == request.MaCTDT))
            {
                return BadRequest(new { message = "Mã chương trình đào tạo này đã tồn tại trong hệ thống!" });
            }

            var newProgram = new Programs
            {
                MaCTDT = request.MaCTDT,
                TenCTDT = request.TenCTDT,
                TrinhDo = request.TrinhDo
            };

            _context.ChuongTrinhDaoTaos.Add(newProgram);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm chương trình đào tạo thành công!", maCTDT = newProgram.MaCTDT });
        }

// api lấy danh sách chương trình đào tạo
        [HttpGet]
        public async Task<IActionResult> GetPrograms()
        {
            var danhSach = await _context.ChuongTrinhDaoTaos.ToListAsync();
            return Ok(danhSach);
        }
    }
}