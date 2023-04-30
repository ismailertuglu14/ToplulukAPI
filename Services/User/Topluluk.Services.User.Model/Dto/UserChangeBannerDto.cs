using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.User.Model.Dto
{
	public class UserChangeBannerDto
	{
		public IFormFile File { get; set; }
	}
}

