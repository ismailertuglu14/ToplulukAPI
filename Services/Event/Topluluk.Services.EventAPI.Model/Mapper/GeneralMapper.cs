using System;
using AutoMapper;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Model.Entity;

namespace Topluluk.Services.EventAPI.Model.Mapper
{
	public class GeneralMapper : Profile
	{
		public GeneralMapper()
		{
			CreateMap<Event, FeedEventDto>();
		}
	}
}

