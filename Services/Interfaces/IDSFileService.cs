using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dsana.Services.Interfaces
{
    public interface IDSFileService
    {
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);

        public string ConvertByteArrayToFile(byte[] fileData, string extension);

        public string GetFileIcon(string file);

        public string FormatFileSize(long bytes);

    }
}
