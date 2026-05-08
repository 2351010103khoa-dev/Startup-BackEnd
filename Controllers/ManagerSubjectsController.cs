using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupBackend.Data;
using StartupBackend.DTOs;
using StartupBackend.Models;

namespace StartupBackend.Controllers
{
    [Route("manager")]
    [ApiController]
    [Authorize]
    public class ManagerSubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManagerSubjectsController(AppDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // NHÓM CHỨC NĂNG: MÔN HỌC
        // ==============================================================================

        // 1. Lấy danh sách môn học (Hỗ trợ tìm kiếm, lọc tình trạng phân công/biên soạn)
        // Endpoint: GET /manager/subjects
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] string? search,
            [FromQuery] string? assign_status,
            [FromQuery] string? compile_status)
        {
            // Join với bảng PhanCongBienSoans để kiểm tra trạng thái phân công
            var query = _context.MonHocs
                .Include(m => m.PhanCongBienSoans)
                .AsQueryable();

            // Tìm kiếm theo tên hoặc mã môn
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(m => m.TenMonHoc.ToLower().Contains(s) || m.MaMonHoc.ToLower().Contains(s));
            }

            // Chuyển dữ liệu sang DTO để tính toán các trường hiển thị trên UI
            var allData = await query.Select(m => new ManagerSubjectResponse
            {
                MaMonHoc = m.MaMonHoc,
                TenMonHoc = m.TenMonHoc,
                SoTinChiLyThuyet = m.SoTinChiLyThuyet,
                SoTinChiThucHanh = m.SoTinChiThucHanh,
                TongSoTinChi = m.SoTinChiLyThuyet + m.SoTinChiThucHanh,
                TrangThaiHoanThanh = m.TrangThaiHoanThanh,
                TinhTrangPhanCong = m.PhanCongBienSoans.Any() ? "Đã phân công" : "Chưa phân công"
            }).ToListAsync();

            // Lọc theo tình trạng phân công (Đã phân công/Chưa phân công)
            if (!string.IsNullOrEmpty(assign_status))
            {
                allData = allData.Where(d => d.TinhTrangPhanCong == assign_status).ToList();
            }

            // Lọc theo tình trạng biên soạn (Hoàn thành/Chưa hoàn thành)
            if (!string.IsNullOrEmpty(compile_status))
            {
                allData = allData.Where(d => d.TrangThaiHoanThanh == compile_status).ToList();
            }

            return Ok(new { data = allData });
        }

        // 2. Tạo môn học mới
        // Endpoint: POST /manager/subjects
        [HttpPost("subjects")]
        public async Task<IActionResult> CreateSubject([FromBody] ManagerSubjectRequest request)
        {
            if (await _context.MonHocs.AnyAsync(m => m.MaMonHoc == request.MaMonHoc))
            {
                return BadRequest(new { message = "Mã môn học này đã tồn tại trong hệ thống!" });
            }

            var newSubject = new Subjects
            {
                MaMonHoc = request.MaMonHoc,
                TenMonHoc = request.TenMonHoc,
                SoTinChiLyThuyet = request.SoTinChiLyThuyet,
                SoTinChiThucHanh = request.SoTinChiThucHanh,
                ChuongTrinhDaoTaoMa = request.ChuongTrinhDaoTaoMa,
                TrangThaiHoanThanh = "Chưa hoàn thành" // Mặc định khi mới tạo
            };

            _context.MonHocs.Add(newSubject);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo môn học thành công!", maMonHoc = newSubject.MaMonHoc });
        }

        // 3. Chi tiết môn học (Dùng để đổ dữ liệu lên form chỉnh sửa)
        // Endpoint: GET /manager/subjects/{id}
        [HttpGet("subjects/{id}")]
        public async Task<IActionResult> GetSubjectDetail(string id)
        {
            var subject = await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMonHoc == id);
            if (subject == null)
            {
                return NotFound(new { message = "Không tìm thấy môn học!" });
            }
            return Ok(subject);
        }

        // 4. Cập nhật thông tin môn học
        // Endpoint: PUT /manager/subjects/{id}
        [HttpPut("subjects/{id}")]
        public async Task<IActionResult> UpdateSubject(string id, [FromBody] ManagerSubjectRequest request)
        {
            var subject = await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMonHoc == id);
            if (subject == null)
            {
                return NotFound(new { message = "Không tìm thấy môn học!" });
            }

            subject.TenMonHoc = request.TenMonHoc;
            subject.SoTinChiLyThuyet = request.SoTinChiLyThuyet;
            subject.SoTinChiThucHanh = request.SoTinChiThucHanh;
            subject.ChuongTrinhDaoTaoMa = request.ChuongTrinhDaoTaoMa;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật môn học thành công!" });
        }

        // 5. Xóa môn học
        // Endpoint: DELETE /manager/subjects/{id}
        [HttpDelete("subjects/{id}")]
        public async Task<IActionResult> DeleteSubject(string id)
        {
            var subject = await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMonHoc == id);
            if (subject == null)
            {
                return NotFound(new { message = "Không tìm thấy môn học!" });
            }

            // Kiểm tra nếu môn này đã được phân công thì không cho xóa (hoặc cảnh báo)
            bool isAssigned = await _context.PhanCongBienSoans.AnyAsync(p => p.MaMonHoc == id);
            if (isAssigned)
            {
                return BadRequest(new { message = "Không thể xóa môn học đã được phân công soạn đề cương!" });
            }

            _context.MonHocs.Remove(subject);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã xóa môn học {id} thành công!" });
        }

        // ==============================================================================
        // NHÓM CHỨC NĂNG: PHÂN CÔNG
        // ==============================================================================

        // 6. Lấy môn chưa phân công (Hỗ trợ tìm kiếm, lọc theo Khoa/Ngành như UI yêu cầu)
        // Endpoint: GET /manager/subjects/unassigned
        [HttpGet("subjects/unassigned")]
        public async Task<IActionResult> GetUnassignedSubjects(
            [FromQuery] string? search,
            [FromQuery] string? maKhoa,
            [FromQuery] string? maCTDT)
        {
            // Lấy danh sách ID các môn đã được phân công
            var assignedIds = await _context.PhanCongBienSoans.Select(p => p.MaMonHoc).ToListAsync();

            // Lấy các môn chưa có trong danh sách phân công
            var query = _context.MonHocs
                .Where(m => !assignedIds.Contains(m.MaMonHoc))
                .AsQueryable();

            // Tìm kiếm theo tên/mã
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(m => m.TenMonHoc.ToLower().Contains(s) || m.MaMonHoc.ToLower().Contains(s));
            }

            // Lọc theo Ngành (Chương trình đào tạo)
            if (!string.IsNullOrEmpty(maCTDT))
            {
                query = query.Where(m => m.ChuongTrinhDaoTaoMa == maCTDT);
            }

            // Lọc theo Khoa (Dựa vào bảng liên kết Khoas -> Programs)
            if (!string.IsNullOrEmpty(maKhoa))
            {
                var programIdsInKhoa = await _context.Khoas
                    .Where(k => k.MaKhoa == maKhoa && k.MaCTDT != null)
                    .Select(k => k.MaCTDT)
                    .ToListAsync();

                query = query.Where(m => programIdsInKhoa.Contains(m.ChuongTrinhDaoTaoMa));
            }

            var result = await query.Select(m => new {
                m.MaMonHoc,
                m.TenMonHoc,
                m.ChuongTrinhDaoTaoMa
            }).ToListAsync();

            return Ok(result);
        }

        // 7. Thực hiện phân công soạn đề cương
        // Endpoint: POST /manager/assignments
        [HttpPost("assignments")]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
        {
            if (request.SubjectIds == null || !request.SubjectIds.Any())
            {
                return BadRequest(new { message = "Vui lòng chọn ít nhất một môn học để phân công!" });
            }

            foreach (var subjectId in request.SubjectIds)
            {
                // Kiểm tra xem môn đã được phân công chưa để tránh trùng lặp
                bool exists = await _context.PhanCongBienSoans.AnyAsync(p => p.MaMonHoc == subjectId);
                if (!exists)
                {
                    var assignment = new PhanCongBienSoan
                    {
                        NguoiBienSoanId = request.CompilerId,
                        MaMonHoc = subjectId,
                        NgayPhanCong = DateTime.UtcNow
                    };
                    _context.PhanCongBienSoans.Add(assignment);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Phân công soạn đề cương thành công!" });
        }
    }
}