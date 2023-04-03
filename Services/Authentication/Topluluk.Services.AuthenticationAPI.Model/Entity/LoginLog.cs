using System;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.AuthenticationAPI.Model.Entity
{
	public class LoginLog : AbstractEntity
	{
		public string UserId { get; set; }
		public string IpAdress { get; set; }
		public string DeviceId { get; set; }
	}
}

