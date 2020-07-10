using Microsoft.EntityFrameworkCore;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class ChatService :IChatService
    {
        private MyDbContext _myDbContext;
        private IRepository<User> _userRepository;

        public ChatService(MyDbContext myDbContext, IRepository<User> userRepository)
        {
            _myDbContext = myDbContext;
            _userRepository = userRepository;
        }

        public async Task SendMessage(int senderId, int reciverId, string message)
        {
            var sender = await _userRepository.GetWhereAsync(x => x.Id == senderId).FirstOrDefaultAsync();
            var revicer = await _userRepository.GetByIdAsync(reciverId);

            //var msg = await GetNewMessage(senderId, reciverId, message);
            //var chat = new Chat{ChatName = "Super" };
            //chat.Messages.Add(msg);
            //sender.Chat = chat;
            //await _myDbContext.AddAsync(chat); 
            //await _myDbContext.SaveChangesAsync();
        }

        private async Task<Message> GetNewMessage(int senderId, int reciverId, string message)
        {
            var msg = new Message { SendId = senderId, ReciveId = reciverId, Text = message };
            _myDbContext.Messages.Add(msg);
            await _myDbContext.SaveChangesAsync();
            return msg;
        }
    }
}
