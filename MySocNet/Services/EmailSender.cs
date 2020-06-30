using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;

namespace MySocNet.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MailAddress from = new MailAddress("owneremailov2002@gmail.com", "ME");
            MailAddress to = new MailAddress(email);
            MailMessage m = new MailMessage(from, to);
            m.Subject = subject;
            m.Body = message;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("owneremailov2002@gmail.com", "565666aqa");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
        }
    }
}
