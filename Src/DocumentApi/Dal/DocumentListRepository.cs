using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using DocumentApi.Infrastructure;

namespace DocumentApi.Dal
{
    public class DocumentListRepository : IDocumentListRepository
    {
        internal static readonly string documentListBlobName = "default";
        internal static readonly string countMetadataKey = "documentCount";

        private readonly IBlobStore blobStore;

        public DocumentListRepository(IBlobStore blobStore) 
        {
            this.blobStore = blobStore;
        }

        public Task<bool> ExistsAsync()
        {
            return this.blobStore.ExistsAsync(documentListBlobName);
        }

        public async Task<IList<Document>> GetAsync()
        {
            List<Document> allDocuments = null;

            using (MemoryStream stream = new MemoryStream())
            {
                await this.blobStore.GetContentAsync(documentListBlobName, stream);

                stream.Position = 0;

                allDocuments = await JsonSerializer.DeserializeAsync<List<Document>>(stream);
            }

            return allDocuments;
        }


        public async Task UpsertAsync(IList<Document> allDocuments)
        {
            BlobHttpHeaders headers = new BlobHttpHeaders()
                {
                    ContentType = MediaTypeNames.Application.Json
                };

            Dictionary<string, string> metadata = new Dictionary<string, string>()
                    {
                        {  countMetadataKey, allDocuments.Count.ToString() }
                    };

            using (MemoryStream stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, allDocuments);

                stream.Position = 0;

                await this.blobStore.UpsertAsync(documentListBlobName, stream, headers, metadata);
            }
        }
    }
}
