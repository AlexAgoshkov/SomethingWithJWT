using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class LastDataService : ILastDataService
    {
        private readonly IRepository<LastChatData> _lastDataRepository;
        private readonly IRepository<UserChat> _userChatRepository;
        
        public LastDataService(IRepository<LastChatData> lastDataRepository, 
            IRepository<UserChat> userChatRepository)
        {
            _lastDataRepository = lastDataRepository;
            _userChatRepository = userChatRepository;
        }

        public async Task AddLastChatData(LastChatData lastChatData)
        {
            var lastdata = await _lastDataRepository
                .GetWhere(x => x.ChatId == lastChatData.ChatId).FirstOrDefaultAsync();
            if (lastdata != null)
            {
                lastdata.UserName = lastChatData.UserName;
                lastdata.Text = lastChatData.Text;
                await _lastDataRepository.UpdateAsync(lastdata);
            }
            else
            {
                await _lastDataRepository.AddAsync(lastChatData);
            }
        }

        public async Task<IList<LastChatData>> GetLastData(int userId)
        {
            var userChatIds = await _userChatRepository.GetWhere(x => x.UserId == userId)
                .Select(x => x.ChatId).ToListAsync();

            return await _lastDataRepository.GetWhere(x => userChatIds.Contains(x.ChatId))
                .Include(x => x.Chat)
                .ToListAsync();
        }
    }
}
