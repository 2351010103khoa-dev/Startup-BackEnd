using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace StartupBackend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            // khởi tạo các thông tin của email được gửi đi
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            // truyền thông tin cấu hình email vào SmtpClient để kết nối và gửi mail
            using var smtp = new SmtpClient();

            smtp.Timeout = 60000; // request time out 60s
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // bỏ qua lỗi chứng chỉ SSL để tránh lỗi khi gửi mail
            try
            {
                string host = _config["EmailSettings:EmailHost"]!;
                int port = int.Parse(_config["EmailSettings:EmailPort"] ?? "587");

                await smtp.ConnectAsync(_config["EmailSettings:EmailHost"], int.Parse(_config["EmailSettings:EmailPort"]), SecureSocketOptions.StartTls);
                
                // xác thực với server smtp băng tài khoản đã đăng ký trong appsettings.json
                await smtp.AuthenticateAsync(_config["EmailSettings:EmailUsername"], _config["EmailSettings:EmailPassword"]);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}