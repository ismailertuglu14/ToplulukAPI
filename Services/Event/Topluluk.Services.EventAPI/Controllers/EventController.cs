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
        public async Task<Response<string>> GetUserEvents()
        {
            return new();
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

    }
}

