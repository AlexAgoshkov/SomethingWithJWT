using MySocNet.InputData;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IAccountActivationService
    {
        Task<ActiveKey> CreateActiveKeyAsync();

        Task<User> UserRegistration(UserRegistration userRegistration, Detect detect);

        Task AddActiveKeyToUserAsync(int userId, int keyId);

        Task ConfirmEmailAsync(string key);
    }
}
