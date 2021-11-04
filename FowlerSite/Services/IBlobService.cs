using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using FowlerSite.Models;

namespace FowlerSite.Services
{
    public interface IBlobService
    {
        public Task<Models.BlobInfo> GetBlobAsync(string name);

        public Task<IEnumerable<string>> ListBlobsAsync();

        public Task UploadFileBlobAsync(string filePath, string fileName);

        public Task UploadContentBlobAsync(string content, string fileName);

        public Task DeleteBlobAsync(string imageName);
    }
}
