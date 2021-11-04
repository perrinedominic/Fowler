using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FowlerSite.Models
{
    public class BlobInfo
    {
        private BlobDownloadInfo value;

        public BlobInfo(BlobDownloadInfo value, string contentType)
        {
            this.value = value;
            ContentType = contentType;
        }

        public Stream Content { get; set; }
        public string ContentType { get; set; }
    }
}
