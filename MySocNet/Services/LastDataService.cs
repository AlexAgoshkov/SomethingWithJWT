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
        private readonly IRepository<Chat> _chatRepository;
        
        public LastDataService(IRepository<LastChatData> lastDataRepository, 
            IRepository<UserChat> userChatRepository,
            IRepository<Chat> chatRepository)
        {
            _lastDataRepository = lastDataRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
        }

        public async Task AddLastChatData(LastChatData lastChatData)
        {
            var lastdata = await _lastDataRepository
                .GetWhere(x => x.ChatId == lastChatData.ChatId).FirstOrDefaultAsync();
            if (lastdata != null)
            {
                var chat = await _lastDataRepository.GetWhere(x => x.ChatId == lastChatData.ChatId).FirstOrDefaultAsync();
                chat.Message = lastChatData.Message;
                chat.User = lastChatData.User;
                await _lastDataRepository.UpdateAsync(chat);
            }
            else
            {
                await _lastDataRepository.AddAsync(lastChatData);
            }
        }

        public async Task<IList<LastChatData>> GetLastData(int userId)
        {
            var userChat = await _userChatRepository.GetWhere(x => x.UserId == userId)
                .Select(x => x.ChatId).ToListAsync();

            return await _lastDataRepository.GetWhere(x => userChat.Contains(x.ChatId.Value))
            .Include(x => x.User)
            .ThenInclude(x => x.UserChats)
            .ThenInclude(x => x.Chat)
            .ThenInclude(x => x.Messages).ToListAsync();
        }
    }
}
