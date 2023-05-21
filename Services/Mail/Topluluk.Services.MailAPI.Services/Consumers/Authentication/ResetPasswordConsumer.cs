using System;
using MassTransit;
using Topluluk.Services.MailAPI.Services.Interface;
using Topluluk.Shared.Messages.Authentication;

namespace Topluluk.Services.MailAPI.Services.Consumers.Authentication
{
    public class ResetPasswordConsumer : IConsumer<ResetPasswordCommand>
    {
        private readonly IMailService _mailService;

        public ResetPasswordConsumer(IMailService mailService)
        {
            _mailService = mailService;
        }

        public async Task Consume(ConsumeContext<ResetPasswordCommand> context)
        {

            await _mailService.SendResetPasswordMail(new()
            {
                To = context.Message.To,
                UserId = context.Message.UserId,
                Code = context.Message.Code
            });
        }
    }
}

