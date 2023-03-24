using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.EventAPI.Model.Dto;
using Topluluk.Services.EventAPI.Services.Interface;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<Response<GetEventByIdDto>> GetEventById(string id)
        {
            return await _eventService.GetEventById(this.UserId, id);
        }

        // todo raw dan form-data ya geçir, resim entegrasyonunu yap.
        // https://localhost:xxxx/event/create
        [HttpPost("create")]
        public async Task<Response<string>> CreateEvent(CreateEventDto dto)
        {
            dto.UserId = this.UserId;
            return await _eventService.CreateEvent(dto);
        }


        // https://localhost:xxxx/event/user/1213123123
        [HttpGet("user/{id}")]
        public async Task<Response<List<FeedEventDto>>> GetUserEvents(string id)
        {
            return await _eventService.GetUserEvents(id);
        }

        [HttpPost("join/{id}")]
        public async Task<Response<string>> JoinEvent(string id)
        {
            return await _eventService.JoinEvent(this.UserId , id);
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

        [HttpPost("create-comment")]
        public async Task<Response<string>> CreateComment(CommentCreateDto dto)
        {
            return await _eventService.CreateComment(this.UserId, dto);
        }

        [HttpGet("{id}/attendees")]
        public async Task<Response<List<GetEventAttendeesDto>>> GetAttendees(string id,int skip, int take)
        {
            return await _eventService.GetEventAttendees(this.UserId, id,skip, take);
        }

        [HttpGet("comments/{id}")]
        public async Task<Response<List<GetEventCommentDto>>> GetComments(string id, int take = 10, int skip = 0)
        {
            return await _eventService.GetEventComments(this.UserId, id, skip, take);
        }
    }
}

