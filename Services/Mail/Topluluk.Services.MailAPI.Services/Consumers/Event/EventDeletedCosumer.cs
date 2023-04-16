using MassTransit;
using Topluluk.Services.MailAPI.Services.Interface;
using Topluluk.Shared.Messages.Event;

namespace Topluluk.Services.MailAPI.Services.Consumers.Event;

public class EventDeletedCosumer : IConsumer<EventDeletedCommand>
{
    private readonly IMailService _mailService;

    public EventDeletedCosumer(IMailService mailService)
    {
        _mailService = mailService;
    }

    public async Task Consume(ConsumeContext<EventDeletedCommand> context)
    {

        await _mailService.EventDeletedMail(new()
        {
            EventName =context.Message.EventName,
            UserMails = context.Message.UserMails,
            UserNames = context.Message.UserNames
        });
    }
}