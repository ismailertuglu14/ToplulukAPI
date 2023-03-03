using System;
namespace Topluluk.Services.User.Model.Dto
{
	public class UserFollowDto
	{
		public string? SourceId { get; set; }
		public string TargetId { get; set; }
	}
}

