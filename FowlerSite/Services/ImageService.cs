using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FowlerSite.Services
{
    public class ImageService
    {
        private BlobContainerClient _client;

        /// <summary>
        /// Create an instance of blob repository
        /// </summary>
        /// <param name="connectionString">The storage account connection string</param>
        /// <param name="containerName">The name of the container</param>
        public ImageService(string connectionString, string containerName)
        {
            _client = new BlobContainerClient(connectionString, containerName);
            // Only create the container if it does not exist
            _client.CreateIfNotExists(PublicAccessType.BlobContainer);
        }

        /// <summary>
        /// Upload a local file to the blob container
        /// </summary>
        /// <param name="localFilePath">Full path to the local file</param>
        /// <param name="pathAndFileName">Full path to the container file</param>
        /// <param name="contentType">The content type of the file being created in the container</param>
        public async Task Upload(string localFilePath, string pathAndFileName, string contentType)
        {
            BlobClient blobClient = _client.GetBlobClient(pathAndFileName);

            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders { ContentType = contentType });
            uploadFileStream.Close();
        }

        /// <summary>
        /// Download file as a string
        /// </summary>
        /// <param name="pathAndFileName">Full path to the container file</param>
        /// <returns>Contents of file as a string</returns>
        public async Task<string> Download(string pathAndFileName)
        {
            BlobClient blobClient = _client.GetBlobClient(pathAndFileName);
            if (await blobClient.ExistsAsync())
            {
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                byte[] result = new byte[download.ContentLength];
                await download.Content.ReadAsync(result, 0, (int)download.ContentLength);

                return Encoding.UTF8.GetString(result);
            }
            return string.Empty;
        }

        /// <summary>
        /// Delete file in container
        /// </summary>
        /// <param name="pathAndFileName">Full path to the container file</param>
        /// <returns>True if file was deleted</returns>
        public async Task<bool> Delete(string pathAndFileName)
        {
            BlobClient blobClient = _client.GetBlobClient(pathAndFileName);
            return await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
