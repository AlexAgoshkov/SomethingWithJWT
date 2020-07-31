using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IEmailService
    {
       Task SendEmailAsync(string email, string subject, string htmlMessage);

        bool IsValidEmail(string email);
    }
}
