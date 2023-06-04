using System;
using Microsoft.AspNetCore.Http;

namespace Topluluk.Services.FileAPI.Model.Dto.Http
{
	public class CoverImageUpdateDto
	{
		public IFormFile File { get; set; }
	}
}

