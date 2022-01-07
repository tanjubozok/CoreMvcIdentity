using System;
using System.Net;
using System.Net.Mail;

namespace CoreMvcIdentity.Helpers
{
    public static class Mail
    {
        public static void Send(string email, string subject, string body)
        {
            try
            {
                SmtpClient smtp = new()
                {
                    Port = 587,
                    Host = "smtp.yandex.com.tr",
                    EnableSsl = true,
                    Credentials = new NetworkCredential("bicogame@yandex.com", "xretmbxlxcimsslb")
                };

                MailMessage mail = new();
                mail.From = new MailAddress("bicogame@yandex.com", subject);
                mail.To.Add(email);
                mail.Subject = subject.Replace("\r\n", "");
                mail.IsBodyHtml = true;
                mail.Body = body;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
