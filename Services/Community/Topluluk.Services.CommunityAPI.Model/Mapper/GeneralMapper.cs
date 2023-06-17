using System;
using AutoMapper;
using Topluluk.Services.CommunityAPI.Model.Dto;
using Topluluk.Services.CommunityAPI.Model.Dto.Http;
using Topluluk.Services.CommunityAPI.Model.Entity;

namespace Topluluk.Services.CommunityAPI.Model.Mapper
{
	public class GeneralMapper :Profile
	{
		public GeneralMapper()
		{
			CreateMap<CommunityCreateDto, Community>().ReverseMap();
			CreateMap<CommunityCreateDto, Community>().ReverseMap();

			CreateMap<Community, CommunitySuggestionMobileDto>();
			CreateMap<Community, CommunitySuggestionWebDto>();
			CreateMap<Community, CommunityGetPreviewDto>();
            CreateMap<Community, CommunityInfoPostLinkDto>();
            CreateMap<UserDtoWithUserName, UserDto>().ReverseMap();

		}
	}
}

