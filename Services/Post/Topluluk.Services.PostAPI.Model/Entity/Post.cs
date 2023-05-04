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
		public bool IsShownOnProfile { get; set; } = true;

        public string? CommunityLink { get; set; }

        public string? EventLink { get; set; }
        
		public Post()
		{
			Files = new List<FileModel>();
		}
	}

	public class FileModel
	{
		public string File { get; set; }
		public FileType Type { get; set; }
		public FileModel(string file)
		{
			File = file;
			Type = GetFileType(Path.GetExtension(File));
		}
		private FileType GetFileType(string fileExtension)
		{
			switch (fileExtension.ToLowerInvariant())
			{
				case ".jpg":
				case ".jpeg":
				case ".png":
				case ".bmp":
				case ".gif":
				case ".webp":
				case ".jfif":	
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

	}

}

