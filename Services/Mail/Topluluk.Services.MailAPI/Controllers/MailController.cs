using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Services.MailAPI.Model.Dtos;
using Topluluk.Services.MailAPI.Model.Dtos.Event;
using Topluluk.Services.MailAPI.Services.Interface;
using Topluluk.Shared;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.MailAPI.Controllers
{

    public class MailController : BaseController
    {

        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }
        
        [HttpPost("successfully-registered")]
        public async Task SendRegisteredMail(MailDto mailDto)
        {
             await _mailService.SendRegisteredMail(mailDto);
        }
        
        [HttpPost("reset-password")]
        public async Task SendRegisteredMail(ResetPasswordDto resetDto)
        {
             await _mailService.SendResetPasswordMail(resetDto);
        }

        [HttpPost("event-deleted")]
        public async Task EventDeleted(EventDeletedDto dto)
        {
            await _mailService.EventDeletedMail(dto);
        }
    }
}

