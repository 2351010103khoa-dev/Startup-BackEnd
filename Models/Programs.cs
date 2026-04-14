using System.ComponentModel.DataAnnotations;

namespace StartupBackend.Models
{
    public class Programs
    {
        [Key]
        public string MaCTDT { get; set; }
        public string TenCTDT { get; set; }
        public string TrinhDo { get; set; }

        // Navigation properties
        public ICollection<Accounts> TaiKhoans { get; set; }
        public ICollection<Subjects> MonHocs { get; set; }
    }
}
