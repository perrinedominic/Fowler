using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FowlerSite.Models;
using FowlerSite.Services;
using System;

namespace FowlerSite.Controllers
{
    [Route("blobs")]
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet("{blobName}")]
        public async Task<IActionResult> GetBlob(string blobName)
        {
            var data = await _blobService.GetBlobAsync(blobName);
            return File(data.Content, data.ContentType);
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListBlobs()
        {
            return Ok(await _blobService.ListBlobsAsync());
        }

        //[HttpPost("uploadfile")]
        //public async Task<IActionResult> UploadFile([FromBody] UploadFileRequest request)
        //{
        //    await _blobService.UploadFileBlobAsync(request.FilePath, request.FileName);
        //    return Ok();
        //}

        //[HttpPost("uploadcontent")]
        //public async Task<IActionResult> UploadContent([FromBody] UploadContentRequest request)
        //{
        //    await _blobService.UploadContentBlobAsync(request.Conent, request.FileName);
        //    return Ok();
        //}

        [HttpDelete("{blobName}")]
        public async Task<IActionResult> DeleteFile(string blobName)
        {
            await _blobService.DeleteBlobAsync(blobName);
            return Ok();
        }
    }
}
