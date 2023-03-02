using System;
using AutoMapper;
using Topluluk.Services.User.Model.Dto;
using _User = Topluluk.Services.User.Model.Entity.User;
namespace Topluluk.Services.User.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<UserInsertDto, _User>().ReverseMap();
		}
	}
}

