using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.PostAPI.Model.Entity
{
	public class Post : AbstractEntity
	{
		public string UserId { get; set; } 
		public string? CommunityId { get; set; }
		public List<FileModel> Files { get; set; }
		public string Description { get; set; }
		public ICollection<InteractionType> Interactions { get; set; }
		public bool IsShownOnProfile { get; set; } = true;

        public string? CommunityLink { get; set; }

        public string? EventLink { get; set; }
        
		public Post()
		{
			Interactions = new HashSet<InteractionType>();
			Files = new List<FileModel>();
		}
	}

	public class FileModel
	{
		public string File { get; set; }
		public FileType Type
		{
			get
			{
				var fileExtension = Path.GetExtension(File).ToLowerInvariant();

				// uzantıya göre dosya türünü belirle
				switch (fileExtension)
				{
					case ".jpg":
					case ".jpeg":
					case ".png":
					case ".bmp":
					case ".gif":
					case ".webp":
						return FileType.IMAGE;
					case ".mp4":
					case ".avi":
					case ".mov":
					case ".wmv":
					case ".flv":
						return FileType.VIDEO;
					default:
						return FileType.OTHER;
				}
			}
			set
			{
				var fileExtension = Path.GetExtension(File)?.ToLowerInvariant();

				// uzantıya göre dosya türünü belirle
				switch (fileExtension)
				{
					case ".jpg":
					case ".jpeg":
					case ".png":
					case ".bmp":
					case ".gif": 
					case ".webp":
						Type = FileType.IMAGE;
						break;
					case ".mp4":
					case ".avi":
					case ".mov":
					case ".wmv":
					case ".flv":
						Type = FileType.VIDEO;
						break;
					default:
						Type = FileType.OTHER; 
						break;
				}
			} 
		}

	}

}

