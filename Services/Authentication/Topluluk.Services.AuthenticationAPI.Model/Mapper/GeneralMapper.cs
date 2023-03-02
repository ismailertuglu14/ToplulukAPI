using System;
using AutoMapper;
using Topluluk.Services.AuthenticationAPI.Model.Dto;
using Topluluk.Services.AuthenticationAPI.Model.Entity;

namespace Topluluk.Services.AuthenticationAPI.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<CreateUserDto, UserCredential>().ReverseMap();
			CreateMap<SignInUserDto, UserCredential>().ReverseMap();
        }
    }

}

