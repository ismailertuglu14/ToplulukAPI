using System;
using Microsoft.AspNetCore.Http;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.FileAPI.Services.Implementation
{
    public class StorageService : IStorageService
    {

        readonly IStorage _storage;

        public string StorageName { get => _storage.GetType().Name; }

        public StorageService(IStorage storage)
        {
            _storage = storage;
        }

        public async Task<List<string>>
                        UploadAsync(string containerName,
                                    IFormFileCollection formFileCollection
                                    )
                    => await _storage.UploadAsync(containerName, formFileCollection);


        public async Task<Response<string>> DeleteAsync(string containerName, string fileName) => await _storage.DeleteAsync(containerName, fileName);


        public List<string> GetFiles(string containerName) => _storage.GetFiles(containerName);

        public bool HasFile(string containerName, string fileName) => _storage.HasFile(containerName, fileName);

        public async Task<Response<string>> UploadOneAsync(string containerName, IFormFile file) => await _storage.UploadOneAsync(containerName, file);
    }
}

