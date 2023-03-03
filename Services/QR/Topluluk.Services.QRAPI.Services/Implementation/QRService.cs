using System;
using QRCoder;
using Topluluk.Services.QRAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;
using System.Drawing;
using static QRCoder.QRCodeGenerator;

namespace Topluluk.Services.QRAPI.Services.Implementation
{
	public class QRService : IQRService
	{

        public async Task<Response<byte[]>> GenerateQRCodeAsync(string text)
        {
            QRCodeGenerator generator = new();
            QRCodeData data = generator.CreateQrCode(text, ECCLevel.Q);
            
            PngByteQRCode qrCode = new(data);
            byte[] byteGraphic = qrCode.GetGraphic(10, new byte[] { 84, 99, 71 }, new byte[] { 240, 240, 240 },true);  
            return await Task.FromResult(Response<byte[]>.Success(byteGraphic,ResponseStatus.Success));
        }
    }
}

