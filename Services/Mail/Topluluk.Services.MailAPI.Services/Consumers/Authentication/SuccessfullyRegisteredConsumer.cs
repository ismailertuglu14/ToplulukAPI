using MassTransit;
using Topluluk.Services.MailAPI.Model.Dtos;
using Topluluk.Services.MailAPI.Services.Interface;
using Topluluk.Shared.Messages;

namespace Topluluk.Services.MailAPI.Services.Consumers
{
	public class SuccessfullyRegisteredConsumer : IConsumer<SuccessfullyRegisteredCommand>
	{
		private readonly IMailService _mailService;
		public SuccessfullyRegisteredConsumer(IMailService mailService)
		{
			_mailService = mailService;
		}


		public async Task Consume(ConsumeContext<SuccessfullyRegisteredCommand> context)
		{
			var to = context.Message.To;
			var fullName = context.Message.FullName;
			MailDto mail = new() { To = to, FullName = fullName };
			await _mailService.SendRegisteredMail(mail);
		}
	}
}

