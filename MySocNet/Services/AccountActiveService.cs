using AutoMapper;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class AccountActiveService : IAccountActivationService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ActiveKey> _activeKeyRepository;
        private readonly IMapper _mapper;

        public AccountActiveService(
            IRepository<User> userRepository,
            IRepository<ActiveKey> activeKeyRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _activeKeyRepository = activeKeyRepository;
            _mapper = mapper;
        }

        public async Task CreateActiveKeyAsync(UserRegistration userRegistration)
        {
            User newUser = GetInitUser(userRegistration);
            await _userRepository.AddAsync(newUser);
            var key = await GenerateActiveKeyAsync();
            var user = await _userRepository.GetWhere(x => x.UserName == newUser.UserName)
                .Include(x => x.ActiveKey).FirstOrDefaultAsync();
            await AddActiveKeyToUserAsync(user.Id, key.Id);
        }

        public async Task ConfirmEmailAsync(string key)
        {
            var activeKey = await _activeKeyRepository.FirstOrDefaultAsync(x => x.Key == key);
            var time = DateTime.Now - activeKey.Created;

            if (!activeKey.IsActive && time.Hours < 1)
            {
                activeKey.IsActive = true;
                await _activeKeyRepository.UpdateAsync(activeKey);
            }
        }

        private User GetInitUser(UserRegistration userRegistration)
        {
            var newUser = _mapper.Map<User>(userRegistration);
            newUser.Password = HashService.Hash(newUser.Password);
            newUser.UserRole = "User";
            return newUser;
        }

        private async Task<ActiveKey> GenerateActiveKeyAsync()
        {
            var activeKey = new ActiveKey { Key = GetRandomString(), Created = DateTime.Now, IsActive = false };
            await _activeKeyRepository.AddAsync(activeKey);
            return activeKey;
        }

        private async Task AddActiveKeyToUserAsync(int userId, int keyId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            user.ActiveKeyId = keyId;
            await _userRepository.UpdateAsync(user);
        }

        private string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
