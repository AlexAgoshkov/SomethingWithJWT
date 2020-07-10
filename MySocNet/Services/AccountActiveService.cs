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
        private readonly MyDbContext _myDbContext;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public AccountActiveService(
            MyDbContext myDbContext, 
            IRepository<User> userRepository,
            IMapper mapper)
        {
            _myDbContext = myDbContext;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task CreateActiveKeyAsync(UserRegistration userRegistration)
        {
            User newUser = GetInitUser(userRegistration);
            await _userRepository.AddAsync(newUser);
            var key = await GenerateActiveKeyAsync();
            var user = await _userRepository.GetWhereAsync(x => x.UserName == newUser.UserName)
                .Include(x => x.ActiveKey).FirstOrDefaultAsync();
            await AddActiveKeyToUserAsync(user.Id, key.Id);
        }

        public async Task ConfirmEmailAsync(string key)
        {
            var activeKey = await _myDbContext.ActiveKeys.FirstOrDefaultAsync(x => x.Key == key);
            var time = DateTime.Now - activeKey.Created;

            if (!activeKey.IsActive && time.Hours < 1)
            {
                activeKey.IsActive = true;
                _myDbContext.ActiveKeys.Update(activeKey);
                await _myDbContext.SaveChangesAsync();
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
            _myDbContext.ActiveKeys.Add(activeKey);
            await _myDbContext.SaveChangesAsync();
            return activeKey;
        }

        private async Task AddActiveKeyToUserAsync(int userId, int keyId)
        {
            var user = await _myDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            user.ActiveKeyId = keyId;
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        private string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
