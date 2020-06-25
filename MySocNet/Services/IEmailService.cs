using MySocNet.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public interface IEmailService
    {
        Task SendEmail(Message message);
    }
}