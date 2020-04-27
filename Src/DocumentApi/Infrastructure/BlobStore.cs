using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentApi.Configuration;

namespace DocumentApi.Infrastructure
{
    public class BlobStore : IBlobStore
    {
        private readonly BlobStorageConfiguration configuration;
        private readonly BlobContainerClient containerClient;

        public BlobStore(BlobStorageConfiguration configuration)
        {
            containerClient = new BlobContainerClient(
                configuration.ConnectionString,
                configuration.ContainerName);
            this.configuration = configuration;
        }        

        public async Task InitAsync()
        {
            try
            {
                await containerClient.CreateIfNotExistsAsync(this.configuration.PublicAccessType);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to configure {containerClient.Name} Blob container", ex);
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            BlobClient client = this.containerClient.GetBlobClient(name);
            Response<bool> response = await client.ExistsAsync();

            return response.Value;
        }

        public async Task UpsertAsync(string name, Stream content, BlobHttpHeaders headers = null, Dictionary<string, string> metaData = null) 
        {
            BlobClient client = this.containerClient.GetBlobClient(name);
            await client.UploadAsync(content, overwrite: true);            

            if (metaData != null) 
            {
                await client.SetMetadataAsync(metaData);
            }

            if (headers != null) 
            {
                await client.SetHttpHeadersAsync(headers);
            }
        }

        public Task GetContentAsync(string name, Stream destinatiionStream)
        {
            BlobClient client = this.containerClient.GetBlobClient(name);

            return client.DownloadToAsync(destinatiionStream);
        }

        public async Task<bool> DeleteAsync(string name)
        {
            BlobClient client = this.containerClient.GetBlobClient(name);

            Response<bool> response = await client.DeleteIfExistsAsync();

            return response.Value;
        }

        public Uri GetDirectDownloadUri(string name)
        {
            BlobUriBuilder uriBuilder = new BlobUriBuilder(this.containerClient.Uri);
            uriBuilder.BlobContainerName = this.containerClient.Name;
            uriBuilder.BlobName = name;

            return uriBuilder.ToUri();
        }
    }
}
