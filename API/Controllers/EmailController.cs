using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System;
using API.Dtos;
using API.Models;
using System.Threading.Tasks;
using MimeKit;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MailKit.Security;
using Microsoft.Extensions.Options;
using API.Helper.Result;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailController(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        [HttpPost("send-random-number")]
        public IActionResult SendRandomNumber([FromBody] EmailRequest request)
        {
            try
            {
                // Tạo số ngẫu nhiên 4 chữ số
                var random = new Random();
                var randomNumber = random.Next(1000, 9999).ToString();

                // Cấu hình SMTP Server
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("vunguyen0106202@gmail.com", "01062002"),

                    EnableSsl = true
                };

                // Tạo email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("vunguyen0106202@gmail.com"),
                    Subject = "Mã xác nhận của bạn",
                    Body = $"Mã xác nhận của bạn là: {randomNumber}",
                    IsBodyHtml = false // Gửi dạng văn bản thuần túy
                };

                // Thêm người nhận
                mailMessage.To.Add(request.To);

                // Gửi email
                smtpClient.Send(mailMessage);

                return Ok(new { Message = "Email đã được gửi thành công!", Code = randomNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> sendemail([FromForm] EmailRequest upload)
        {
            try
            {
                var email = new MimeMessage();
                //email.Sender = new MailboxAddress("Nguyễn Vũ", "vunguyen0106202@gmail.com");
                //email.From.Add(new MailboxAddress("Nguyễn Vũ", "vunguyen0106202@gmail.com"));
                email.Sender = new MailboxAddress("Nguyễn Vũ", _smtpSettings.Username);
                email.From.Add(new MailboxAddress("Nguyễn Vũ", _smtpSettings.Username));
                email.To.Add(new MailboxAddress(upload.To, upload.To));
                email.Subject = "Krik";
                email.Body = new TextPart("plain")
                {
                    Text = $"Mã của bạn là: {upload.Ma}"
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                //smtp.Authenticate("vunguyen0106202@gmail.com", "wchc dany uojn igvn");
                smtp.Authenticate(_smtpSettings.Username, _smtpSettings.Password);


                smtp.Send(email);
                smtp.Disconnect(true);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
        [HttpPost("outlook")]
        public async Task<ActionResult> sendoutlook([FromForm] EmailRequest upload)
        {
            try
            {
                var email = new MimeMessage();

                // Thông tin người gửi
                email.Sender = new MailboxAddress("Nguyen Long Vu D20CN02", "vunguyen0106202@outlock.com.vn");
                email.From.Add(new MailboxAddress("Nguyen Long Vu D20CN02", "vunguyen0106202@outlock.com.vn"));

                // Thông tin người nhận
                email.To.Add(new MailboxAddress(upload.To, upload.To));
                email.Subject = "Hello from Outlook";

                // Nội dung email
                email.Body = new TextPart("plain")
                {
                    Text = "Nội dung email của bạn tại đây"
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                // Bật TLS 1.2 để đảm bảo bảo mật
                System.Net.ServicePointManager.SecurityProtocol =
                    System.Net.SecurityProtocolType.Tls12;

                // Kết nối tới SMTP server của Outlook
                await smtp.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);

                // Xác thực người dùng
                await smtp.AuthenticateAsync("vunguyen0106202@outlock.com.vn", "Vu01062002@");

                // Gửi email
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return Ok("Email đã gửi thành công qua Outlook!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi gửi email: {ex.Message}");
            }
        }

    }
}
