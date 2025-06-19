using System.Net.Mail;

namespace SistemaRestaurante.Utilities
{
    internal class EmailBuilder
    {
        private MailMessage _message = new();

        public EmailBuilder From(string from)
        {
            _message.From = new MailAddress(from);
            return this;
        }

        public EmailBuilder To(string to)
        {
            _message.To.Add(to);
            return this;
        }

        public EmailBuilder Subject(string subject)
        {
            _message.Subject = subject;
            return this;
        }

        public EmailBuilder Body(string body, bool isHtml = true)
        {
            _message.Body = body;
            _message.IsBodyHtml = isHtml;
            return this;
        }

        public EmailBuilder Attach(string filePath)
        {
            _message.Attachments.Add(new Attachment(filePath));
            return this;
        }

        public MailMessage Build() => _message;
    }
}
