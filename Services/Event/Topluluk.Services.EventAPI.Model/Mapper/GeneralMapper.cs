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
			CreateMap<Event, EventDto>().ForMember(d => d.AttendeesCount, s => s.MapFrom(s => s.Attendees.Count));
			CreateMap<CommentCreateDto, EventComment>();
			CreateMap<EventComment, GetEventCommentDto>();
			CreateMap<GetUserInfoDto, GetEventCommentDto>().ReverseMap(); // ?
			CreateMap<Event, GetEventAttendeesDto>();
			CreateMap<EventAttendee, GetEventAttendeesDto>().ForMember(d => d.Id, s => s.MapFrom(s => s.UserId));
            CreateMap<CreateEventDto, Event>();
            
		}
	}
}

