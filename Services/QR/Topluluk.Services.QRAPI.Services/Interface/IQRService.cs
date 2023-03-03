using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.QRAPI.Services.Interface
{
	public interface IQRService
	{
		Task<Response<byte[]>> GenerateQRCodeAsync(string text);
	}
}

