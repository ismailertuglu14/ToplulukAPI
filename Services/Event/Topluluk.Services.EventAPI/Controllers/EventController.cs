using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Controllers
{
    public class EventController : BaseController
    {

        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{id}")]
        public async Task<Response<EventDto>> GetEventById(string id)
        {
            return await _eventService.GetEventById(this.UserId, this.Token, id);
        }
        
        [HttpPost("create")]
        public async Task<Response<string>> CreateEvent([FromForm]CreateEventDto dto)
        {
            return await _eventService.CreateEvent(this.UserId, this.Token, dto);
        }

        [HttpGet("user/{id}")]
        public async Task<Response<List<EventDto>>> GetUserEvents(string id)
        {
            return await _eventService.GetUserEvents(id,this.Token);
        }

        [HttpPost("join/{id}")]
        public async Task<Response<string>> JoinEvent(string id)
        {
            return await _eventService.JoinEvent(this.UserId , id);
        }
        
        [HttpPost("leave/{id}")]
        public async Task<Response<NoContent>> LeaveEvent(string id)
        {
            return await _eventService.LeaveEvent(this.UserId , id);
        }
        
        [HttpPost("delete")]
        public async Task<Response<string>> DeleteEvent(string id)
        {
            return await _eventService.DeleteEvent(this.UserId, id);
        }

        [HttpPost("delete-completely")]
        public async Task<Response<string>> DeleteEventCompletely(string id)
        {
            return await _eventService.DeleteCompletelyEvent(this.UserId, id);
        }
        [HttpPost("expire")]
        public async Task<Response<string>> EventExpire(string id)
        {
            return await _eventService.ExpireEvent(this.UserId, id);
        }


        [HttpGet("{id}/attendees")]
        public async Task<Response<List<GetEventAttendeesDto>>> GetAttendees(string id,int skip, int take)
        {
            return await _eventService.GetEventAttendees(this.Token, id,skip, take);
        }

        
    }
}

