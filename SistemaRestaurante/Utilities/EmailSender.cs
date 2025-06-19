using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace SistemaRestaurante.Utilities
{
    internal class EmailSender
    {
        private readonly SmtpClient _smtp;
        private readonly string _from;

        public EmailSender(IConfiguration configuration)
        {
            var section = configuration.GetSection("EmailSettings");
            var settings = new EmailSettings
            {
                Host = section["Host"],
                Port = int.Parse(section["Port"]),
                Username = section["Username"],
                Password = section["Password"],
                EnableSsl = bool.Parse(section["EnableSsl"]),
                From = section["From"]
            };


            _smtp = new SmtpClient(settings.Host)
            {
                Port = settings.Port,
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                EnableSsl = settings.EnableSsl
            };

            _from = settings.From;
        }

        public void Send(string to, string subject, string body)
        {
            var message = new MailMessage(_from, to, subject, body)
            {
                IsBodyHtml = true
            };

            _smtp.Send(message);
        }
    }
}
