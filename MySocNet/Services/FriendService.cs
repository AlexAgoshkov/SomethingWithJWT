using AutoMapper;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class FriendService : IFriendService
    {
        private readonly IRepository<Friend> _friendRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public FriendService(
            IRepository<Friend> friendRepository, 
            IRepository<User> userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _friendRepository = friendRepository;
            _mapper = mapper;
        }

        public async Task AddFriendToUserAsync(int userId, int friendId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var friend = new Friend { UserAddedId = friendId, UserId = userId };

                user.Friends.Add(friend);
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<IList<UserResponse>> GetFriendListAsync(int userId)
        {
            var friendIds = await _friendRepository.GetWhere(
                  x => x.UserId == userId || x.Id == userId)
                 .Select(x => x.UserId == userId ? x.Id : x.UserId)
                 .ToListAsync();

            var friends = await _userRepository.GetWhere(
                x => friendIds.Contains(x.Id))
                .ToListAsync();

            return _mapper.Map<IList<UserResponse>>(friends);
        }
    }
}