using System;
namespace Topluluk.Services.FileAPI.Services.Interface
{
	public interface IStorageService : IStorage
	{
		public string StorageName { get; }
	}
}

