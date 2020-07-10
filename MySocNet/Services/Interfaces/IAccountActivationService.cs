using MySocNet.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IAccountActivationService
    {
        Task CreateActiveKeyAsync(UserRegistration userRegistration);

        Task ConfirmEmailAsync(string key);
    }
}
