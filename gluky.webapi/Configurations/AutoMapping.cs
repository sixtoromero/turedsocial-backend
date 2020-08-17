using AutoMapper;
using gluky.webapi.Dtos;
using gluky.webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gluky.webapi.Configurations
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<users, usersDto>().ReverseMap();
            CreateMap<posts, postsDto>().ReverseMap();
        }
    }
}
