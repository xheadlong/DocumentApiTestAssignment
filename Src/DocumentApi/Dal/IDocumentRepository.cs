using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentApi.Dal
{
    public interface IDocumentRepository
    {
        Task<string> AddAsync(string mimeType, string userFileName, Stream content);

        Task<bool> RemoveAsync(string id);

        Uri GetDirectDownloadUri(string id);
    }
}
