using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using DocumentApi.Infrastructure;

namespace DocumentApi.Dal
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IBlobStore blobStore;

        public DocumentRepository(IBlobStore blobStore)
        {
            this.blobStore = blobStore;
        }

        public async Task<string> AddAsync(string mimeType, string userFileName, Stream content)
        {
            string id = Guid.NewGuid().ToString("N");

            BlobHttpHeaders headers = new BlobHttpHeaders()
                {
                    ContentType = mimeType,
                    ContentDisposition = $"attachment; filename = \"{userFileName}\""
                };

            await this.blobStore.UpsertAsync(id, content, headers);

            return id;
        }

        public Task<bool> RemoveAsync(string id)
        {
            return this.blobStore.DeleteAsync(id);
        }

        public Uri GetDirectDownloadUri(string id) 
        {
            return this.blobStore.GetDirectDownloadUri(id);
        }
        
    }


}
