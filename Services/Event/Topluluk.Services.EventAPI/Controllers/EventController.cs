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


        [HttpPost("create")]
        public async Task<Response<string>> CreateEvent(CreateEventDto dto)
        {
            dto.UserId = this.UserId;
            return await _eventService.CreateEvent(dto);
        }
        
    }
}

