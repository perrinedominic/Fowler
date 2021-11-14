using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using FowlerSite.Models;
using Microsoft.AspNetCore.Http;

namespace FowlerSite.Services
{
    public interface IBlobService
    {
        public Task<Models.BlobInfo> GetBlobAsync(string name);

        public Task<IEnumerable<string>> ListBlobsAsync();

        public Task UploadContentBlobAsync(string content, string fileName);

        public Task UploadFileBlobAsync(IFormFile image);

        public Task DeleteBlobAsync(string imageName);
    }
}
