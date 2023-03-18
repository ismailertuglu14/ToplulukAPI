using System;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Services.Interface
{
	public interface IEventService
	{
		Task<Response<string>> CreateEvent(CreateEventDto dto);
	}
}

