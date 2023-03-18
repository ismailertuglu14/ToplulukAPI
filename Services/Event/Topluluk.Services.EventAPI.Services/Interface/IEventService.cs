using System;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Services.Interface
{
	public interface IEventService
	{
		Task<Response<string>> CreateEvent(CreateEventDto dto);
		Task<Response<string>> GetUserEvents(string id);
		Task<Response<string>> GetEventSuggestions();
		Task<Response<string>> GetEventById(string userId, string id);
        Task<Response<string>> ExpireEvent(string userId, string id);
        Task<Response<string>> DeleteEvent(string userId, string id);
		Task<Response<string>> DeleteCompletelyEvent(string userId, string id);
    }
}

