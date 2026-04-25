namespace StartupBackend.DTOs
{
    public class AccountDTOs
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Khoa { get; set; } = string.Empty;
    }
}
