using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;

namespace DocumentApi.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAllAsync();

        Task<string> AddAsync(string userFileName, string mimeType, ByteSize size, Stream content);

        Task<bool> RemoveAsync(string id);

        Task UpdateDocumentOrderAsync(IEnumerable<string> documentIdSetInNewOrder);
    }
}
