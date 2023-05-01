using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Topluluk.Services.FileAPI.Services.Interface;
using Topluluk.Shared.Dtos;
using Topluluk.Shared.Enums;

namespace Topluluk.Services.FileAPI.Services.Implementation
{
	public class AzureStorage : Storage, IAzureStorage
    {

        readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _blobContainerClient;
        
        public AzureStorage(IConfiguration configuration)
		{
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }

        public async Task<List<string>> UploadAsync(string containerName, IFormFileCollection formFileCollection)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<string> datas = new();
            foreach (IFormFile file in formFileCollection)
            {
                string fileNewName = await FileRenameAsync(containerName, file.FileName, HasFile);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNewName);
                await blobClient.UploadAsync(file.OpenReadStream());
                datas.Add(blobClient.Uri.AbsoluteUri);
            }
            return datas;
        }
        public async Task<Response<string>> DeleteAsync(string containerName, string fileName)
        {
            try
            {
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                string fileNameWithExtension = fileName.Substring(fileName.LastIndexOf("/") + 1);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNameWithExtension);
                var a =  await blobClient.DeleteAsync();
                return await Task.FromResult(Response<string>.Success("Successfully deleted", ResponseStatus.Success));
            }
            catch (Exception e)
            {
                return await Task.FromResult(Response<string>.Fail($"Some error occurred: {e}", ResponseStatus.InitialError));

            }
        }

        public List<string> GetFiles(string containerName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Select(b => b.Name).ToList();
        }

        public bool HasFile(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Any(b => b.Name == fileName);
        }

        public async Task<Response<string>> UploadOneAsync(string containerName, IFormFile file)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

  
            string fileNewName = await FileRenameAsync(containerName, file.FileName, HasFile);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileNewName);
            await blobClient.UploadAsync(file.OpenReadStream());
                  
            return await Task.FromResult(Response<string>.Success(blobClient.Uri.AbsoluteUri,ResponseStatus.Success));
        }
    }
}

