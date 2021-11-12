using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace FowlerSite.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        private IWebHostEnvironment hostingEnv;

        public BlobService(BlobServiceClient blobServiceClient, IWebHostEnvironment env)
        {
            this._blobServiceClient = blobServiceClient;
            this.hostingEnv = env;
        }

        public Task DeleteBlobAsync(string imageName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Models.BlobInfo> GetBlobAsync(string name)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("gameimages");
            var blobClient = containerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadAsync();

            return new Models.BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
        }

        public async Task<IEnumerable<string>> ListBlobsAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("gameimages");
            var items = new List<string>();

            await foreach(var blobItem in containerClient.GetBlobsAsync())
            {
                items.Add(blobItem.Name);
            }

            return items;
        }

        public async Task UploadContentBlobAsync(string content, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("gameimages");
            var blobClient = containerClient.GetBlobClient(fileName);

            var bytes = Encoding.UTF8.GetBytes(content);
            await using var memoryStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(memoryStream);
        }

        /// <summary>
        /// Uploads image to azure storage.
        /// </summary>
        /// <param name="image">The image to be uploaded.</param>
        /// <returns></returns>
        public async Task UploadFileBlobAsync(IFormFile image)
        {
            // Gets the blob container and client.
            var containerClient = _blobServiceClient.GetBlobContainerClient("gameimages");
            var blobClient = containerClient.GetBlobClient(image.FileName);

            // Gets the file path for the image saved to the project files.
            var fileDirectory = "assets/images";
            string FilePath = Path.Combine(hostingEnv.WebRootPath, fileDirectory);
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);
            var fileName = image.FileName;
            var filePath = Path.Combine(FilePath, fileName);

            await blobClient.UploadAsync(filePath);
        }
    }
}
