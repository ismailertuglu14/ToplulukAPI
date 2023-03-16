using System;
using AutoMapper;
using Topluluk.Services.CommunityAPI.Model.Dto;
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
			CreateMap<Community,CommunitySuggestionWebDto>().ForMember(x => x.ParticipiantCount, o => o.MapFrom(x => x.Participiants.Count));

			CreateMap<Community, CommunityGetPreviewDto>().ForMember(x => x.ParticipiantsCount, o => o.MapFrom(x => x.Participiants.Count)); 
        }
	}
}

