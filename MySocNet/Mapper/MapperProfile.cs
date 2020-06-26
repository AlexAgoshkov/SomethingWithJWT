﻿using AutoMapper;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.OutPutData;
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
            CreateMap<User, UserOutPut>();
            CreateMap<UserLogin, User>();
            CreateMap<UserUpdate, User>();
            CreateMap<User, UserUpdate>();
        }
    }
}