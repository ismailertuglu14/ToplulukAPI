using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.FileAPI.Services.Interface
{
	public interface IStorage
	{
        Task<List<string>> UploadAsync(string containerName, IFormFileCollection formFileCollection);
        Task<Response<string>> UploadOneAsync(string containerName, IFormFile file);

        Task<Response<string>> DeleteAsync(string containerName, string fileName);

        List<string> GetFiles(string containerName);

        bool HasFile(string containerName, string fileName);
    }
}

