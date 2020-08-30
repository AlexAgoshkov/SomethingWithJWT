using AutoMapper;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using MySocNet.Exceptions;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class AccountActiveService : IAccountActivationService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ActiveKey> _activeKeyRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        
        public AccountActiveService(
            IRepository<User> userRepository,
            IRepository<ActiveKey> activeKeyRepository,
            IEmailService emailService,
            IMapper mapper)
            {
                _userRepository = userRepository;
                _activeKeyRepository = activeKeyRepository;
                _emailService = emailService;
                _mapper = mapper;
            }

        public async Task<ActiveKey> CreateActiveKeyAsync()
        {
            var activeKey = new ActiveKey { Key = GetRandomString(), Created = DateTime.Now, IsActive = false };
            await _activeKeyRepository.AddAsync(activeKey);
            return activeKey;
        }

        public async Task ConfirmEmailAsync(string key)
        {
            var activeKey = await _activeKeyRepository.FirstOrDefaultAsync(x => x.Key == key);
            if (activeKey == null)
                throw new EntityNotFoundException("ActiveKey not found");

            var time = DateTime.Now - activeKey.Created;

            if (!activeKey.IsActive)
            {
                activeKey.IsActive = true;
                await _activeKeyRepository.UpdateAsync(activeKey);
            }
        }

        public async Task<User> UserRegistration(UserRegistration userRegistration, Detect detect)
        {
            var newUser = _mapper.Map<User>(userRegistration);
            newUser.Password = HashService.Hash(newUser.Password);
            newUser.UserRole = "User";
            newUser.Detects.Add(detect);
            await _userRepository.AddAsync(newUser);

            var key = await CreateActiveKeyAsync();

            await AddActiveKeyToUserAsync(newUser.Id, key.Id);

            return newUser;
        }

        public async Task AddActiveKeyToUserAsync(int userId, int keyId)
        {
            var user = await _userRepository.GetWhere(x => x.Id == userId)
                .Include(x => x.ActiveKey).FirstOrDefaultAsync();
            user.ActiveKeyId = keyId;
            await _userRepository.UpdateAsync(user);
        }

        private string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
