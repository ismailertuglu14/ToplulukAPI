using System;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Services.Interface
{
	public interface IEventService
	{
		Task<Response<string>> CreateEvent(string userId, string token, CreateEventDto dto);
		Task<Response<List<FeedEventDto>>> GetUserEvents(string id);
		Task<Response<string>> GetEventSuggestions();
		Task<Response<GetEventByIdDto>> GetEventById(string userId, string id);

        Task<Response<string>> JoinEvent(string userId, string eventId);
        Task<Response<NoContent>> LeaveEvent(string userId, string eventId);

		Task<Response<string>> ExpireEvent(string userId, string id);
        Task<Response<string>> DeleteEvent(string userId, string id);
		Task<Response<string>> DeleteCompletelyEvent(string userId, string id);

		Task<Response<string>> CreateComment(string userId, CommentCreateDto dto);
		Task<Response<List<GetEventCommentDto>>> GetEventComments(string userId, string id, int skip = 0, int take = 10);

        Task<Response<List<GetEventAttendeesDto>>> GetEventAttendees(string userId, string eventId, int skip = 0,
            int take = 10);
    }
}

