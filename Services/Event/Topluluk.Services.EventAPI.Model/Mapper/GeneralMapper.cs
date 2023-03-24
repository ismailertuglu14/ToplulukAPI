using System;
using AutoMapper;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Model.Dto.Http;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<Event, FeedEventDto>();
			CreateMap<Event, GetEventByIdDto>().ForMember(d => d.AttendeesCount, s => s.MapFrom(s => s.Attendees.Count));
			CreateMap<CommentCreateDto, EventComment>();
			CreateMap<EventComment, GetEventCommentDto>().ForMember(d => d.InteractionCount, s => s.MapFrom(s => s.Interactions.Count));
			CreateMap<GetUserInfoDto, GetEventCommentDto>().ReverseMap(); // ?
            CreateMap<Event, GetEventAttendeesDto>();
        }
	}
}

