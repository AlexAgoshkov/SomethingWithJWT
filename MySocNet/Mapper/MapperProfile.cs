using AutoMapper;
using MySocNet.Input;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<UserLogin, User>();
            CreateMap<UserUpdate, User>();
            CreateMap<User, UserUpdate>();
            CreateMap<UserRegistration, User>();
            CreateMap<Chat, ChatLastResponse>();
            CreateMap<Chat, ChatResponse>();
            CreateMap<Authentication, UpdateTokenInput>();
            CreateMap<UpdateTokenInput, Authentication>();
            CreateMap<InputChatCreate, Chat>();
            CreateMap<Message, MessageResponse>()
                .ForMember(x => x.SenderId, y => y.MapFrom(x => x.SenderId));
            CreateMap<LastChatData, ChatLastResponse>()
                .ForMember(response => response.Id, opt => opt.MapFrom(lastChatData => lastChatData.ChatId))
                .ForMember(x => x.SenderName, opt => opt.MapFrom(x => x.UserName))
                .ForMember(x => x.LastMessage, opt => opt.MapFrom(x => x.Text));
            CreateMap<Message, LastChatData>()
                .ForMember(response => response.ChatId, opt => opt.MapFrom(x => x.ChatId))
                .ForMember(response => response.UserName, opt => opt.MapFrom(x => x.Sender.FirstName + x.Sender.SurName))
                .ForMember(response => response.Text, opt => opt.MapFrom(x => x.Text));
        }
    }
}
